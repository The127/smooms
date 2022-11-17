using NodaTime;

namespace smooms.api.Services;

public interface IClockService
{
    DateTimeZone TimeZone { get; }
    
    Instant Now { get; }
    LocalDateTime LocalNow { get; }
    
    Instant ToInstant(LocalDateTime localDateTime);
    Instant ToInstant(LocalDate localDate);
    LocalDateTime ToLocalDateTime(Instant instant);

    Instant? ToInstant(LocalDateTime? localDateTime);
    Instant? ToInstant(LocalDate? localDate);
    LocalDateTime? ToLocalDateTime(Instant? instant);
}

public class ClockService : IClockService
{
    private readonly IClock _clock;
    
    public ClockService(IClock clock, DateTimeZone timeZone)
    {
        TimeZone = timeZone;
        _clock = clock;
    }

    public DateTimeZone TimeZone { get; }
    
    public Instant Now => SystemClock.Instance.GetCurrentInstant();
    public LocalDateTime LocalNow => ToLocalDateTime(Now);
    
    public Instant ToInstant(LocalDateTime localDateTime)
    {
        return localDateTime.InZoneLeniently(TimeZone).ToInstant();
    }

    public Instant ToInstant(LocalDate localDate)
    {
        return localDate.AtMidnight().InZoneLeniently(TimeZone).ToInstant();
    }

    public LocalDateTime ToLocalDateTime(Instant instant)
    {
        return instant.InZone(TimeZone).LocalDateTime;
    }

    public Instant? ToInstant(LocalDateTime? localDateTime)
    {
        return localDateTime.HasValue ? ToInstant(localDateTime.Value) : null;
    }

    public Instant? ToInstant(LocalDate? localDate)
    {
        return localDate.HasValue ? ToInstant(localDate.Value) : null;
    }

    public LocalDateTime? ToLocalDateTime(Instant? instant)
    {
        return instant.HasValue ? ToLocalDateTime(instant.Value) : null;
    }
}