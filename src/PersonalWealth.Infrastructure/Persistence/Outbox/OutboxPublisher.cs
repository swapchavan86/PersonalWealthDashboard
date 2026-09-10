using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersonalWealth.Application.Events;

namespace PersonalWealth.Infrastructure.Persistence.Outbox;

public sealed class OutboxPublisher(
    PersonalWealthDbContext dbContext,
    IEventBus eventBus,
    IOptions<OutboxPublisherOptions> options,
    ILogger<OutboxPublisher> logger)
{
    private readonly OutboxPublisherOptions options = options.Value;

    public async Task<int> PublishPendingAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var messages = await dbContext.OutboxMessages
            .Where(x => x.PublishedAtUtc == null &&
                        x.AttemptCount < options.MaxAttempts &&
                        (x.NextAttemptAtUtc == null || x.NextAttemptAtUtc <= now))
            .OrderBy(x => x.CreatedAtUtc)
            .Take(options.BatchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var eventType = Type.GetType(message.EventType, throwOnError: true)
                    ?? throw new InvalidOperationException($"Event type '{message.EventType}' could not be loaded.");
                var domainEvent = JsonSerializer.Deserialize(message.Payload, eventType) as PersonalWealth.Domain.Events.IDomainEvent
                    ?? throw new InvalidOperationException($"Payload for event '{message.EventId}' could not be deserialized.");

                await eventBus.PublishAsync(domainEvent, cancellationToken);
                message.MarkPublished(now);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                var nextAttempt = now.AddSeconds(options.RetryDelaySeconds * Math.Max(1, message.AttemptCount + 1));
                message.RecordFailure(exception.Message, nextAttempt);
                logger.LogError(exception, "Failed to publish outbox event {EventId}; retry scheduled for {NextAttemptUtc}.", message.EventId, nextAttempt);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return messages.Count;
    }
}
