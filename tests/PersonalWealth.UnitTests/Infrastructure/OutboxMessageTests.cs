using PersonalWealth.Infrastructure.Persistence.Outbox;
using Xunit;

namespace PersonalWealth.UnitTests.Infrastructure;

public sealed class OutboxMessageTests
{
    [Fact]
    public void Failure_state_increments_attempt_and_sets_retry_metadata()
    {
        var message = OutboxMessage.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test.Event",
            "{}",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);
        var retryAt = DateTimeOffset.UtcNow.AddMinutes(1);

        message.RecordFailure("temporary failure", retryAt);

        Assert.Equal(1, message.AttemptCount);
        Assert.Equal("temporary failure", message.LastError);
        Assert.Equal(retryAt, message.NextAttemptAtUtc);
        Assert.Null(message.PublishedAtUtc);
    }

    [Fact]
    public void Published_state_clears_failure_metadata()
    {
        var message = OutboxMessage.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test.Event",
            "{}",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);
        message.RecordFailure("temporary failure", DateTimeOffset.UtcNow.AddMinutes(1));
        var publishedAt = DateTimeOffset.UtcNow.AddMinutes(2);

        message.MarkPublished(publishedAt);

        Assert.Equal(publishedAt, message.PublishedAtUtc);
        Assert.Null(message.LastError);
        Assert.Null(message.NextAttemptAtUtc);
    }
}
