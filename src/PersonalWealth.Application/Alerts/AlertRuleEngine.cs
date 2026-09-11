using PersonalWealth.Domain.Alerts;

namespace PersonalWealth.Application.Alerts;

public sealed class AlertRuleEngine : IAlertRuleEngine
{
    public IReadOnlyList<AlertEvaluation> Evaluate(IEnumerable<AlertDefinition> definitions, FinancialAlertContext context)
    {
        ArgumentNullException.ThrowIfNull(definitions);
        return definitions
            .Where(x => x.Status == AlertStatus.Active)
            .Select(x => EvaluateOne(x, context))
            .ToArray();
    }

    private static AlertEvaluation EvaluateOne(AlertDefinition definition, FinancialAlertContext context)
    {
        var (triggered, actual) = definition.ConditionType switch
        {
            AlertConditionType.NetWorthBelow => (context.NetWorth < definition.Threshold, context.NetWorth),
            AlertConditionType.CashBelow => (context.Cash < definition.Threshold, context.Cash),
            AlertConditionType.DebtAbove => (context.Debt > definition.Threshold, context.Debt),
            AlertConditionType.SavingsRateBelow => (context.SavingsRate < definition.Threshold, context.SavingsRate),
            _ => throw new ArgumentOutOfRangeException()
        };

        var message = $"{definition.Name}: current value {actual:0.##}, threshold {definition.Threshold:0.##}.";
        return new AlertEvaluation(definition.Id, definition.TenantId, definition.Name, definition.Severity, triggered, message);
    }
}
