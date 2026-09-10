using Microsoft.EntityFrameworkCore;
using PersonalWealth.Application.Events;

namespace PersonalWealth.Infrastructure.Persistence.ProcessedEvents;

public sealed class EfCoreProcessedEventStore(PersonalWealthDbContext dbContext) : IProcessedEventStore
{
    public Task<bool> HasProcessedAsync(Guid eventId, CancellationToken cancellationToken = default) =>
        dbContext.ProcessedEvents.AnyAsync(x => x.EventId == eventId, cancellationToken);

    public async Task MarkProcessedAsync(
        Guid eventId,
        string eventType,
        DateTimeOffset processedAtUtc,
        CancellationToken cancellationToken = default)
    {
        if (await HasProcessedAsync(eventId, cancellationToken))
        {
            return;
        }

        dbContext.ProcessedEvents.Add(ProcessedEvent.Create(eventId, eventType, processedAtUtc));
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
