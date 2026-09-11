using PersonalWealth.Application.Alerts;
using PersonalWealth.Domain.Alerts;
using Xunit;

namespace PersonalWealth.UnitTests.Alerts;

public sealed class NotificationDeliveryServiceTests
{
    [Fact]
    public async Task Retries_transient_failures_and_records_attempts()
    {
        var provider = new FlakyProvider(2);
        var store = new InMemoryStore();
        var service = new NotificationDeliveryService(provider, store);
        var notification = new Notification("n-1", Guid.NewGuid(), AlertSeverity.Warning, "Cash is low", DateTime.UtcNow);

        var result = await service.DeliverAsync([notification]);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, provider.Calls);
        Assert.Equal(3, store.GetAttempts("n-1").Count);
        Assert.True(store.GetAttempts("n-1").Last().Succeeded);
    }

    [Fact]
    public async Task Does_not_redeliver_a_successful_notification()
    {
        var provider = new FlakyProvider(0);
        var store = new InMemoryStore();
        var service = new NotificationDeliveryService(provider, store);
        var notification = new Notification("n-2", Guid.NewGuid(), AlertSeverity.Info, "Done", DateTime.UtcNow);

        await service.DeliverAsync([notification]);
        await service.DeliverAsync([notification]);

        Assert.Equal(1, provider.Calls);
    }

    private sealed class FlakyProvider(int failures) : INotificationProvider
    {
        private readonly int failures = failures;
        public int Calls { get; private set; }
        public Task DeliverAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            Calls++;
            if (Calls <= failures) throw new InvalidOperationException("temporary failure");
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryStore : INotificationDeliveryStore
    {
        private readonly List<DeliveryAttempt> attempts = [];
        public bool HasSucceeded(string notificationId) => attempts.Any(x => x.NotificationId == notificationId && x.Succeeded);
        public void Record(DeliveryAttempt attempt) => attempts.Add(attempt);
        public IReadOnlyList<DeliveryAttempt> GetAttempts(string notificationId) => attempts.Where(x => x.NotificationId == notificationId).ToArray();
    }
}
