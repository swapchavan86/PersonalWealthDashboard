using PersonalWealth.Application.Alerts;
using PersonalWealth.Domain.Alerts;
using Xunit;

namespace PersonalWealth.UnitTests.Alerts;

public sealed class AlertRuleEngineTests
{
    [Fact]
    public void Evaluates_thresholds_deterministically()
    {
        var tenant = Guid.NewGuid();
        var definitions = new[]
        {
            new AlertDefinition(Guid.NewGuid(), tenant, "Cash warning", AlertConditionType.CashBelow, 10000m, AlertSeverity.Warning),
            new AlertDefinition(Guid.NewGuid(), tenant, "Debt critical", AlertConditionType.DebtAbove, 50000m, AlertSeverity.Critical)
        };

        var result = new AlertRuleEngine().Evaluate(definitions, new FinancialAlertContext(200000m, 5000m, 75000m, 12m));

        Assert.Equal(2, result.Count);
        Assert.All(result, x => Assert.True(x.Triggered));
    }

    [Fact]
    public void Ignores_resolved_and_disabled_alerts()
    {
        var tenant = Guid.NewGuid();
        var resolved = new AlertDefinition(Guid.NewGuid(), tenant, "Resolved", AlertConditionType.CashBelow, 100m, AlertSeverity.Info);
        resolved.Resolve();
        var disabled = new AlertDefinition(Guid.NewGuid(), tenant, "Disabled", AlertConditionType.CashBelow, 100m, AlertSeverity.Info);
        disabled.Disable();

        var result = new AlertRuleEngine().Evaluate([resolved, disabled], new FinancialAlertContext(0m, 0m, 0m, 0m));

        Assert.Empty(result);
    }
}
