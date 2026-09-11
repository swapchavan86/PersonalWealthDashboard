using System.Collections.Concurrent;
using PersonalWealth.Application.Alerts;

namespace PersonalWealth.Infrastructure.Alerts;

public sealed class InMemoryNotificationDeliveryStore : INotificationDeliveryStore
{
    private readonly ConcurrentDictionary<string, List<DeliveryAttempt>> attempts = new(StringComparer.Ordinal);
    public bool HasSucceeded(string notificationId) => attempts.TryGetValue(notificationId, out var values) && values.Any(x => x.Succeeded);
    public void Record(DeliveryAttempt attempt) => attempts.AddOrUpdate(attempt.NotificationId, _ => [attempt], (_, values) => { lock (values) values.Add(attempt); return values; });
    public IReadOnlyList<DeliveryAttempt> GetAttempts(string notificationId) => attempts.TryGetValue(notificationId, out var values) ? values.ToArray() : [];
}
