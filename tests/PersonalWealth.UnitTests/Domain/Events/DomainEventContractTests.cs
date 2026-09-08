using PersonalWealth.Domain.Events;
using Xunit;

namespace PersonalWealth.UnitTests.Domain.Events;

public sealed class DomainEventContractTests
{
    [Fact]
    public void Event_identity_and_occurrence_metadata_are_exposed()
    {
        var eventId = Guid.NewGuid();
        var occurredAtUtc = new DateTimeOffset(2026, 9, 8, 6, 0, 0, TimeSpan.Zero);
        IDomainEvent domainEvent = new TestEvent(eventId, occurredAtUtc);

        Assert.Equal(eventId, domainEvent.EventId);
        Assert.Equal(occurredAtUtc, domainEvent.OccurredAtUtc);
    }

    [Fact]
    public void Event_metadata_remains_stable_after_creation()
    {
        var domainEvent = new TestEvent(
            Guid.NewGuid(),
            new DateTimeOffset(2026, 9, 8, 6, 0, 0, TimeSpan.Zero));

        var firstEventId = domainEvent.EventId;
        var firstOccurredAtUtc = domainEvent.OccurredAtUtc;

        Assert.Equal(firstEventId, domainEvent.EventId);
        Assert.Equal(firstOccurredAtUtc, domainEvent.OccurredAtUtc);
    }

    [Fact]
    public void Contract_is_implementable_without_infrastructure_types()
    {
        IDomainEvent domainEvent = new TestEvent(
            Guid.NewGuid(),
            new DateTimeOffset(2026, 9, 8, 6, 0, 0, TimeSpan.Zero));

        Assert.IsType<TestEvent>(domainEvent);
        Assert.Equal(typeof(TestEvent).Namespace, "PersonalWealth.UnitTests.Domain.Events");
    }

    private sealed record TestEvent(Guid EventId, DateTimeOffset OccurredAtUtc) : IDomainEvent;
}
