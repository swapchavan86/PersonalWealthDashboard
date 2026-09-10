using PersonalWealth.Application.Events;
using PersonalWealth.Domain.Events;
using Xunit;

namespace PersonalWealth.UnitTests.Application.Events;

public sealed class InProcessEventBusTests
{
    [Fact]
    public async Task Handlers_execute_in_the_supplied_order()
    {
        var calls = new List<int>();
        var domainEvent = new TestEvent(Guid.NewGuid(), DateTimeOffset.UtcNow);
        var bus = CreateBus(
            domainEvent,
            (_, _) => { calls.Add(1); return Task.CompletedTask; },
            (_, _) => { calls.Add(2); return Task.CompletedTask; });

        await bus.PublishAsync(domainEvent);

        Assert.Equal([1, 2], calls);
    }

    [Fact]
    public async Task Handler_failure_stops_dispatch_and_propagates_the_exception()
    {
        var calls = new List<int>();
        var expected = new InvalidOperationException("handler failed");
        var domainEvent = new TestEvent(Guid.NewGuid(), DateTimeOffset.UtcNow);
        var bus = CreateBus(
            domainEvent,
            (_, _) => { calls.Add(1); return Task.FromException(expected); },
            (_, _) => { calls.Add(2); return Task.CompletedTask; });

        var actual = await Assert.ThrowsAsync<InvalidOperationException>(
            () => bus.PublishAsync(domainEvent));

        Assert.Same(expected, actual);
        Assert.Equal([1], calls);
    }

    [Fact]
    public async Task Cancellation_is_observed_between_handlers()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var calls = new List<int>();
        var domainEvent = new TestEvent(Guid.NewGuid(), DateTimeOffset.UtcNow);
        var bus = CreateBus(
            domainEvent,
            (_, _) => { calls.Add(1); cancellationTokenSource.Cancel(); return Task.CompletedTask; },
            (_, _) => { calls.Add(2); return Task.CompletedTask; });

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => bus.PublishAsync(domainEvent, cancellationTokenSource.Token));

        Assert.Equal([1], calls);
    }

    [Fact]
    public async Task Events_without_handlers_are_a_no_op()
    {
        var bus = new InProcessEventBus(
            new Dictionary<Type, IReadOnlyList<Func<IDomainEvent, CancellationToken, Task>>>());

        await bus.PublishAsync(new TestEvent(Guid.NewGuid(), DateTimeOffset.UtcNow));
    }

    private static InProcessEventBus CreateBus(
        TestEvent domainEvent,
        params Func<IDomainEvent, CancellationToken, Task>[] handlers) =>
        new(new Dictionary<Type, IReadOnlyList<Func<IDomainEvent, CancellationToken, Task>>>
        {
            [domainEvent.GetType()] = handlers
        });

    private sealed record TestEvent(Guid EventId, DateTimeOffset OccurredAtUtc) : IDomainEvent;
}
