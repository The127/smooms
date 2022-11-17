using System.Text;
using FluentValidation;
using HttpExceptions;
using Microsoft.EntityFrameworkCore;
using smooms.api.Models;
using smooms.api.Services;
using smooms.api.Utils;

namespace smooms.api.Commands;

public record CreateUserCommand(
    string DisplayName,
    string EMail,
    string Password
) : CommandBase<CreateUserResponse>;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(UserConfiguration.UserNameMaxLength);

        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(x => x.EMail)
            .EmailAddressOrEmpty();
    }
}

public record CreateUserResponse
{
    public required UserId UserId { get; init; }
}

public class CreateUserCommandHandler : CommandHanderBase<CreateUserCommand, CreateUserResponse>
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ISecurityService _securityService;

    public CreateUserCommandHandler(IDbContextFactory<AppDbContext> dbContextFactory, ISecurityService securityService)
    {
        _dbContextFactory = dbContextFactory;
        _securityService = securityService;
    }

    public override async Task<CreateUserResponse> Handle(CreateUserCommand request,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var doesAccountWithEmailExist = await dbContext.Set<User>()
            .AnyAsync(x => x.Email == request.EMail, cancellationToken);

        if (doesAccountWithEmailExist)
            throw new HttpConflictException("Account with this email already exists");

        var salt = _securityService.GenerateSalt();
        var passwordBytes = Encoding.UTF8.GetBytes(request.Password);
        var hashedPassword = _securityService.HashPassword(passwordBytes, salt);

        var account = new User
        {
            UserName = request.DisplayName,
            Email = request.EMail,
            HashedPassword = hashedPassword,
            Salt = salt,
        };

        await dbContext.AddAsync(account, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new CreateUserResponse()
        {
            UserId = account.Id,
        };

        return response;
    }
}