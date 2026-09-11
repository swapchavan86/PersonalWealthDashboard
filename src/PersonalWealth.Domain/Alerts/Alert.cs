using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Domain.Alerts;

public enum AlertSeverity { Info, Warning, Critical }
public enum AlertStatus { Active, Resolved, Disabled }
public enum AlertConditionType { NetWorthBelow, CashBelow, DebtAbove, SavingsRateBelow }

public sealed class AlertDefinition : TenantEntity<Guid>, IAuditableEntity
{
    private AlertDefinition() : base(Guid.NewGuid(), Guid.NewGuid()) { }
    public AlertDefinition(Guid id, Guid tenantId, string name, AlertConditionType conditionType, decimal threshold, AlertSeverity severity) : base(id, tenantId)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (threshold < 0) throw new ArgumentOutOfRangeException(nameof(threshold));
        Name = name.Trim(); ConditionType = conditionType; Threshold = threshold; Severity = severity; Status = AlertStatus.Active;
    }
    public string Name { get; private set; } = null!;
    public AlertConditionType ConditionType { get; private set; }
    public decimal Threshold { get; private set; }
    public AlertSeverity Severity { get; private set; }
    public AlertStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public void Resolve() => Status = AlertStatus.Resolved;
    public void Disable() => Status = AlertStatus.Disabled;
    public void Activate() => Status = AlertStatus.Active;
}

public sealed record AlertEvaluation(Guid AlertId, Guid TenantId, string AlertName, AlertSeverity Severity, bool Triggered, string Message);
