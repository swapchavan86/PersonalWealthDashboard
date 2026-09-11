using System.Collections.Concurrent;
using PersonalWealth.Application.Alerts;

namespace PersonalWealth.Infrastructure.Alerts;

public sealed class DevelopmentNotificationProvider : INotificationProvider
{
    private readonly ConcurrentQueue<Notification> delivered = new();
    public Task DeliverAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        delivered.Enqueue(notification);
        return Task.CompletedTask;
    }
    public IReadOnlyList<Notification> GetDelivered() => delivered.ToArray();
}
