using PersonalWealth.Application.Results;
using PersonalWealth.Domain.Alerts;

namespace PersonalWealth.Application.Alerts;

public sealed record FinancialAlertContext(decimal NetWorth, decimal Cash, decimal Debt, decimal SavingsRate);
public sealed record Notification(string Id, Guid TenantId, AlertSeverity Severity, string Message, DateTime OccurredAt);
public sealed record DeliveryAttempt(string NotificationId, int Attempt, bool Succeeded, string? Error, DateTime AttemptedAt);

public interface IAlertRuleEngine
{
    IReadOnlyList<AlertEvaluation> Evaluate(IEnumerable<AlertDefinition> definitions, FinancialAlertContext context);
}

public interface INotificationProvider
{
    Task DeliverAsync(Notification notification, CancellationToken cancellationToken = default);
}

public interface INotificationDeliveryStore
{
    bool HasSucceeded(string notificationId);
    void Record(DeliveryAttempt attempt);
    IReadOnlyList<DeliveryAttempt> GetAttempts(string notificationId);
}

public interface IAlertNotificationService
{
    Task<Result<IReadOnlyList<DeliveryAttempt>>> DeliverAsync(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default);
}
