using PersonalWealth.Application.Results;

namespace PersonalWealth.Application.Alerts;

public sealed class NotificationDeliveryService : IAlertNotificationService
{
    private const int MaxAttempts = 3;
    private readonly INotificationProvider provider;
    private readonly INotificationDeliveryStore store;

    public NotificationDeliveryService(INotificationProvider provider, INotificationDeliveryStore store)
    {
        this.provider = provider;
        this.store = store;
    }

    public async Task<Result<IReadOnlyList<DeliveryAttempt>>> DeliverAsync(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default)
    {
        var attempts = new List<DeliveryAttempt>();
        foreach (var notification in notifications)
        {
            if (store.HasSucceeded(notification.Id)) continue;
            for (var attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                try
                {
                    await provider.DeliverAsync(notification, cancellationToken);
                    var record = new DeliveryAttempt(notification.Id, attempt, true, null, DateTime.UtcNow);
                    store.Record(record); attempts.Add(record); break;
                }
                catch (Exception ex) when (attempt < MaxAttempts)
                {
                    var record = new DeliveryAttempt(notification.Id, attempt, false, ex.Message, DateTime.UtcNow);
                    store.Record(record); attempts.Add(record);
                }
                catch (Exception ex)
                {
                    var record = new DeliveryAttempt(notification.Id, attempt, false, ex.Message, DateTime.UtcNow);
                    store.Record(record); attempts.Add(record);
                    return Result<IReadOnlyList<DeliveryAttempt>>.Failure(new ApplicationError("notification_delivery_failed", ex.Message));
                }
            }
        }
        return Result<IReadOnlyList<DeliveryAttempt>>.Success(attempts);
    }
}
