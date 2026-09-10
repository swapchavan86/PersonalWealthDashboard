using PersonalWealth.Domain.Events;

namespace PersonalWealth.Application.Events;

public sealed class InProcessEventBus : IEventBus
{
    private readonly IReadOnlyDictionary<Type, IReadOnlyList<Func<IDomainEvent, CancellationToken, Task>>> handlers;

    public InProcessEventBus(
        IReadOnlyDictionary<Type, IReadOnlyList<Func<IDomainEvent, CancellationToken, Task>>> handlers)
    {
        ArgumentNullException.ThrowIfNull(handlers);
        this.handlers = handlers;
    }

    public async Task PublishAsync(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        if (!handlers.TryGetValue(domainEvent.GetType(), out var eventHandlers))
        {
            return;
        }

        foreach (var handler in eventHandlers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await handler(domainEvent, cancellationToken);
        }
    }
}
