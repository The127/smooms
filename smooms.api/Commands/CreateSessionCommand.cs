using System.Text;
using FluentValidation;
using HttpExceptions;
using Microsoft.EntityFrameworkCore;
using smooms.api.Models;
using smooms.api.Services;

namespace smooms.api.Commands;

public record CreateSessionCommand(
    string Email,
    string Password
) : CommandBase<CreateSessionResponse>;

public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}

public record CreateSessionResponse
{
    public required SessionId SessionId { get; set; }
}

public class CreateSessionCommandHandler : CommandHanderBase<CreateSessionCommand, CreateSessionResponse>
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ISecurityService _securityService;
    private readonly IClockService _clockService;
    
    public CreateSessionCommandHandler(IDbContextFactory<AppDbContext> dbContextFactory,
        ISecurityService securityService, IClockService clockService)
    {
        _dbContextFactory = dbContextFactory;
        _securityService = securityService;
        _clockService = clockService;
    }

    public override async Task<CreateSessionResponse> Handle(CreateSessionCommand request,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var userInfo = await dbContext.Set<User>()
            .Where(x => x.Email == request.Email)
            .Select(x => new
            {
                x.Id,
                x.Salt,
                x.HashedPassword,
            }).FirstOrDefaultAsync(cancellationToken);

        if (userInfo is null)
            throw new HttpUnauthorizedException();

        var passwordBytes = Encoding.UTF8.GetBytes(request.Password);
        var hashedPassword = _securityService.HashPassword(passwordBytes, userInfo.Salt);
        if (!hashedPassword.SequenceEqual(userInfo.HashedPassword))
            throw new HttpUnauthorizedException();

        var now = _clockService.Now;
        var session = new Session
        {
            UserId = userInfo.Id,
            CreatedAt = now,
            LastAccessedAt = now,
        };

        await dbContext.AddAsync(session, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        var response = new CreateSessionResponse
        {
            SessionId = session.Id,
        };
        return response;
    }
}