namespace PersonalWealth.Application.Events;

public interface IProcessedEventStore
{
    Task<bool> HasProcessedAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task MarkProcessedAsync(Guid eventId, string eventType, DateTimeOffset processedAtUtc, CancellationToken cancellationToken = default);
}
