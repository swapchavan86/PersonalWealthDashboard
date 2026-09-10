using PersonalWealth.Application.Events;
using PersonalWealth.Domain.Events;
using Xunit;

namespace PersonalWealth.UnitTests.Application.Events;

public sealed class IdempotentEventBusTests
{
    [Fact]
    public async Task Duplicate_event_is_ignored_after_successful_processing()
    {
        var store = new InMemoryProcessedEventStore();
        var eventId = Guid.NewGuid();
        var calls = 0;
        var bus = new InProcessEventBus(
            new Dictionary<Type, IReadOnlyList<Func<IDomainEvent, CancellationToken, Task>>>
            {
                [typeof(TestEvent)] = [(_, _) => { calls++; return Task.CompletedTask; }]
            },
            store);

        var domainEvent = new TestEvent(eventId, DateTimeOffset.UtcNow);
        await bus.PublishAsync(domainEvent);
        await bus.PublishAsync(domainEvent);

        Assert.Equal(1, calls);
        Assert.True(await store.HasProcessedAsync(eventId));
    }

    [Fact]
    public async Task Failed_handler_does_not_mark_event_processed()
    {
        var store = new InMemoryProcessedEventStore();
        var bus = new InProcessEventBus(
            new Dictionary<Type, IReadOnlyList<Func<IDomainEvent, CancellationToken, Task>>>
            {
                [typeof(TestEvent)] = [(_, _) => throw new InvalidOperationException("handler failed")]
            },
            store);

        var domainEvent = new TestEvent(Guid.NewGuid(), DateTimeOffset.UtcNow);
        await Assert.ThrowsAsync<InvalidOperationException>(() => bus.PublishAsync(domainEvent));
        Assert.False(await store.HasProcessedAsync(domainEvent.EventId));
    }

    private sealed record TestEvent(Guid EventId, DateTimeOffset OccurredAtUtc) : IDomainEvent;

    private sealed class InMemoryProcessedEventStore : IProcessedEventStore
    {
        private readonly HashSet<Guid> ids = [];

        public Task<bool> HasProcessedAsync(Guid eventId, CancellationToken cancellationToken = default) =>
            Task.FromResult(ids.Contains(eventId));

        public Task MarkProcessedAsync(
            Guid eventId,
            string eventType,
            DateTimeOffset processedAtUtc,
            CancellationToken cancellationToken = default)
        {
            ids.Add(eventId);
            return Task.CompletedTask;
        }
    }
}
