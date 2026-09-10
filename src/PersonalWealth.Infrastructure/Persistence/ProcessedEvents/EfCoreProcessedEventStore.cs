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

        var processedEvent = ProcessedEvent.Create(eventId, eventType, processedAtUtc);
        dbContext.ProcessedEvents.Add(processedEvent);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException) when (await HasProcessedAsync(eventId, cancellationToken))
        {
            dbContext.Entry(processedEvent).State = EntityState.Detached;
        }
    }
}
