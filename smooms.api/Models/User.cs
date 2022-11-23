using System.ComponentModel.DataAnnotations;
using CheckConstraints;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace smooms.api.Models;

public record UserId : EntityIdBase<UserId>;

public class User : EntityBase<UserId>
{
    public required string UserName { get; init; } = null!;
    
    [EmailAddress]
    public required string Email { get; init; }

    public required byte[] HashedPassword { get; init; } = null!;
    public required byte[] Salt { get; init; } = null!;

    public List<Session> Sessions { get; set; } = new();
}

public class UserConfiguration : EntityBaseConfiguration<User, UserId>
{
    public const int UserNameMaxLength = 100;
    public const int UserNameMinLength = 5;
    public const int EmailMaxLength = 320;

    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.UserName)
            .HasLengthConstraint(UserNameMaxLength, UserNameMinLength);

        builder.Property(x => x.Email)
            .HasLengthConstraint(EmailMaxLength)
            .HasEmailConstraint();
    }
}