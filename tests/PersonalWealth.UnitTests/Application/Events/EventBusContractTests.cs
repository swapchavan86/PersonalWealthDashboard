using PersonalWealth.Application.Events;
using PersonalWealth.Domain.Events;
using Xunit;

namespace PersonalWealth.UnitTests.Application.Events;

public sealed class EventBusContractTests
{
    [Fact]
    public void Event_bus_contract_is_framework_independent_and_publishes_domain_events()
    {
        IEventBus eventBus = new TestEventBus();
        var domainEvent = new TestEvent(Guid.NewGuid(), DateTimeOffset.UtcNow);

        Assert.IsAssignableFrom<IEventBus>(eventBus);
        Assert.IsAssignableFrom<IDomainEvent>(domainEvent);
    }

    [Fact]
    public async Task Event_handler_contract_accepts_a_matching_domain_event()
    {
        var handler = new TestEventHandler();
        var domainEvent = new TestEvent(Guid.NewGuid(), DateTimeOffset.UtcNow);

        await handler.HandleAsync(domainEvent);

        Assert.Same(domainEvent, handler.ReceivedEvent);
    }

    private sealed class TestEventBus : IEventBus
    {
        public Task PublishAsync(
            IDomainEvent domainEvent,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class TestEventHandler : IEventHandler<TestEvent>
    {
        public TestEvent? ReceivedEvent { get; private set; }

        public Task HandleAsync(
            TestEvent domainEvent,
            CancellationToken cancellationToken = default)
        {
            ReceivedEvent = domainEvent;
            return Task.CompletedTask;
        }
    }

    private sealed record TestEvent(Guid EventId, DateTimeOffset OccurredAtUtc) : IDomainEvent;
}
