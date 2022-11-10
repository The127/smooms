using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace smooms.app.Models;

public record AccountId : EntityIdBase<AccountId>;

public class Account : EntityBase<AccountId>
{
    [StringLength(AccountConfiguration.DisplayNameMaxLength)]
    public required string DisplayName { get; init; } 
    
    [StringLength(AccountConfiguration.EMailMaxLength)]
    [EmailAddress]
    public required string? EMail { get; init; }
    public required bool EMailVerified { get; init; }

    public required byte[] HashedPassword { get; init; }
    public required byte[] Salt { get; init; }
    
    // this is a one-to-many relationship
    // its the owning side
    // you can use this navigation property to get all the roles of this account
    public List<Device> Devices { get; init; } = new();
}

public class AccountConfiguration : EntityBaseConfiguration<Account, AccountId>
{
    public const int DisplayNameMaxLength = 1_000;
    public const int EMailMaxLength = 320;

    public override void Configure(EntityTypeBuilder<Account> builder)
    {
        base.Configure(builder);
        
        builder.HasIndex(x => x.EMail).IsUnique();
    }
}