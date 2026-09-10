using PersonalWealth.Domain.Events;

namespace PersonalWealth.Application.Events;

public sealed class EventHandlerRegistry
{
    private readonly Dictionary<Type, List<Func<IDomainEvent, CancellationToken, Task>>> handlers = [];
    private readonly Dictionary<Type, HashSet<object>> registeredHandlers = [];

    public void Register<TEvent>(IEventHandler<TEvent> handler)
        where TEvent : IDomainEvent
    {
        ArgumentNullException.ThrowIfNull(handler);

        var eventType = typeof(TEvent);
        if (!registeredHandlers.TryGetValue(eventType, out var eventHandlers))
        {
            eventHandlers = [];
            registeredHandlers[eventType] = eventHandlers;
            handlers[eventType] = [];
        }

        if (!eventHandlers.Add(handler))
        {
            throw new InvalidOperationException(
                $"Handler '{handler.GetType().Name}' is already registered for event '{eventType.Name}'.");
        }

        handlers[eventType].Add((domainEvent, cancellationToken) =>
            handler.HandleAsync((TEvent)domainEvent, cancellationToken));
    }

    public IEventBus CreateEventBus(IProcessedEventStore? processedEventStore = null) =>
        new InProcessEventBus(
            handlers.ToDictionary(
                pair => pair.Key,
                pair => (IReadOnlyList<Func<IDomainEvent, CancellationToken, Task>>)[..pair.Value]),
            processedEventStore);
}
