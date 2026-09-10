using PersonalWealth.Domain.Events;

namespace PersonalWealth.Application.Events;

public sealed class InProcessEventBus : IEventBus
{
    private readonly IReadOnlyDictionary<Type, IReadOnlyList<Func<IDomainEvent, CancellationToken, Task>>> handlers;
    private readonly IProcessedEventStore? processedEventStore;

    public InProcessEventBus(
        IReadOnlyDictionary<Type, IReadOnlyList<Func<IDomainEvent, CancellationToken, Task>>> handlers,
        IProcessedEventStore? processedEventStore = null)
    {
        ArgumentNullException.ThrowIfNull(handlers);
        this.handlers = handlers;
        this.processedEventStore = processedEventStore;
    }

    public async Task PublishAsync(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        if (processedEventStore is not null &&
            await processedEventStore.HasProcessedAsync(domainEvent.EventId, cancellationToken))
        {
            return;
        }

        if (handlers.TryGetValue(domainEvent.GetType(), out var eventHandlers))
        {
            foreach (var handler in eventHandlers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await handler(domainEvent, cancellationToken);
            }
        }

        if (processedEventStore is not null)
        {
            await processedEventStore.MarkProcessedAsync(
                domainEvent.EventId,
                domainEvent.GetType().AssemblyQualifiedName ?? domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
                DateTimeOffset.UtcNow,
                cancellationToken);
        }
    }
}
