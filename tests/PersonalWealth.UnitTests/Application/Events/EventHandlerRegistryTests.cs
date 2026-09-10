using PersonalWealth.Application.Events;
using PersonalWealth.Domain.Events;
using Xunit;

namespace PersonalWealth.UnitTests.Application.Events;

public sealed class EventHandlerRegistryTests
{
    [Fact]
    public async Task Explicit_registration_dispatches_multiple_handlers_in_registration_order()
    {
        var calls = new List<int>();
        var registry = new EventHandlerRegistry();
        registry.Register(new RecordingHandler(1, calls));
        registry.Register(new RecordingHandler(2, calls));

        await registry.CreateEventBus().PublishAsync(new TestEvent(Guid.NewGuid(), DateTimeOffset.UtcNow));

        Assert.Equal([1, 2], calls);
    }

    [Fact]
    public void Registering_the_same_handler_instance_twice_fails_clearly()
    {
        var registry = new EventHandlerRegistry();
        var handler = new RecordingHandler(1, []);

        registry.Register(handler);

        var exception = Assert.Throws<InvalidOperationException>(() => registry.Register(handler));

        Assert.Contains("already registered", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Null_handler_registration_fails_safely()
    {
        var registry = new EventHandlerRegistry();

        Assert.Throws<ArgumentNullException>(() => registry.Register<TestEvent>(null!));
    }

    private sealed class RecordingHandler(int value, List<int> calls) : IEventHandler<TestEvent>
    {
        public Task HandleAsync(TestEvent domainEvent, CancellationToken cancellationToken = default)
        {
            calls.Add(value);
            return Task.CompletedTask;
        }
    }

    private sealed record TestEvent(Guid EventId, DateTimeOffset OccurredAtUtc) : IDomainEvent;
}
