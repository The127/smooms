using System.Text;
using FluentValidation;
using HttpExceptions;
using Microsoft.EntityFrameworkCore;
using smooms.app.Models;
using smooms.app.Services;
using smooms.app.Utils;

namespace smooms.app.Commands;

public record CreateAccountCommand(
        string DisplayName,
        string Password,
        string EMail
    )
    : CommandBase<CreateAccountResponse>;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(AccountConfiguration.DisplayNameMaxLength);

        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(x => x.EMail)
            .EmailAddressOrEmpty();
    }
}

public record CreateAccountResponse
{
    public required AccountId AccountId { get; init; } = null!;
}

public class RegisterCommandHandler : CommandHanderBase<CreateAccountCommand, CreateAccountResponse>
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ISecurityService _securityService;

    public RegisterCommandHandler(IDbContextFactory<AppDbContext> dbContextFactory, ISecurityService securityService)
    {
        _dbContextFactory = dbContextFactory;
        _securityService = securityService;
    }

    public override async Task<CreateAccountResponse> Handle(CreateAccountCommand request,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var doesAccountWithEmailExist = await dbContext.Set<Account>()
            .AnyAsync(x => x.EMail == request.EMail, cancellationToken);

        if(doesAccountWithEmailExist)
            throw new HttpConflictException("Account with this email already exists");

        var salt = _securityService.GenerateSalt();
        var passwordBytes = Encoding.UTF8.GetBytes(request.Password);
        var hashedPassword = _securityService.HashPassword(passwordBytes, salt);

        var account = new Account
        {
            DisplayName = request.DisplayName,
            EMail = request.EMail,
            HashedPassword = hashedPassword,
            Salt = salt,
            EMailVerified = false,
        };

        await dbContext.AddAsync(account, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new CreateAccountResponse()
        {
            AccountId = account.Id,
        };
        
        return response;
    }
}