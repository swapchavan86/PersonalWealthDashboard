namespace PersonalWealth.Infrastructure.Persistence.ProcessedEvents;

public sealed class ProcessedEvent
{
    private ProcessedEvent(Guid eventId, string eventType, DateTimeOffset processedAtUtc)
    {
        EventId = eventId;
        EventType = eventType;
        ProcessedAtUtc = processedAtUtc;
    }

    private ProcessedEvent()
    {
    }

    public Guid EventId { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public DateTimeOffset ProcessedAtUtc { get; private set; }

    public static ProcessedEvent Create(Guid eventId, string eventType, DateTimeOffset processedAtUtc)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException("EventId must not be empty.", nameof(eventId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(eventType);
        return new ProcessedEvent(eventId, eventType, processedAtUtc);
    }
}
