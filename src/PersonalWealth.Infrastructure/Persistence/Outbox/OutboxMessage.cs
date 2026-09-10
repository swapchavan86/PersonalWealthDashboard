namespace PersonalWealth.Infrastructure.Persistence.Outbox;

public sealed class OutboxMessage
{
    private OutboxMessage(
        Guid eventId,
        Guid? tenantId,
        string eventType,
        string payload,
        DateTimeOffset occurredAtUtc,
        DateTimeOffset createdAtUtc)
    {
        EventId = eventId;
        TenantId = tenantId;
        EventType = eventType;
        Payload = payload;
        OccurredAtUtc = occurredAtUtc;
        CreatedAtUtc = createdAtUtc;
    }

    private OutboxMessage()
    {
    }

    public Guid EventId { get; private set; }

    public Guid? TenantId { get; private set; }

    public string EventType { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset? PublishedAtUtc { get; private set; }

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public static OutboxMessage Create(
        Guid eventId,
        Guid? tenantId,
        string eventType,
        string payload,
        DateTimeOffset occurredAtUtc,
        DateTimeOffset createdAtUtc)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException("EventId must not be empty.", nameof(eventId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(eventType);
        ArgumentNullException.ThrowIfNull(payload);

        return new OutboxMessage(
            eventId,
            tenantId,
            eventType,
            payload,
            occurredAtUtc,
            createdAtUtc);
    }
}
