using NodaTime;

namespace smooms.api.Models;

public record SessionId : EntityIdBase<SessionId>;

public class Session : EntityBase<SessionId>
{
    public required UserId UserId { get; set; }
    public User User { get; set; } = null!;

    public required Instant CreatedAt { get; set; }
    public required Instant LastAccessedAt { get; set; }
}

public class SessionConfiguration : EntityBaseConfiguration<Session, SessionId>
{
    
}