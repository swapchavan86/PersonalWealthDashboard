using PersonalWealth.Application.AI;
using Xunit;

namespace PersonalWealth.UnitTests.AI;

public sealed class FinancialContextBuilderTests
{
    [Fact]
    public void Wealth_builder_is_deterministic_and_minimized()
    {
        var builder = new WealthAiContextBuilder();
        var context = new WealthAiContext(100m, 200m, 50m, 75m, 275m, 0.25m);

        var first = builder.Build(context);
        var second = builder.Build(context);

        Assert.Equal(first, second);
        Assert.Equal("{\"cash\":100,\"investments\":200,\"otherAssets\":50,\"liabilities\":75,\"netWorth\":275,\"savingsRate\":0.25}", first);
        Assert.DoesNotContain("tenant", first, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Investment_builder_orders_records_and_normalizes_symbol()
    {
        var builder = new InvestmentAiContextBuilder();
        var input = new[]
        {
            new InvestmentAiContext(" msft ", 2m, 100m, 120m),
            new InvestmentAiContext("aapl", 1m, 80m, 90m)
        };

        var json = builder.Build(input);

        Assert.True(json.IndexOf("AAPL", StringComparison.Ordinal) < json.IndexOf("MSFT", StringComparison.Ordinal));
        Assert.DoesNotContain("msft ", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Expense_builder_uses_stable_date_format_and_order()
    {
        var builder = new ExpenseAiContextBuilder();
        var input = new[]
        {
            new ExpenseAiContext(new DateTime(2026, 2, 1, 17, 30, 0), 50m, "Food"),
            new ExpenseAiContext(new DateTime(2026, 1, 1), 25m, "Travel")
        };

        var json = builder.Build(input);

        Assert.True(json.IndexOf("2026-01-01", StringComparison.Ordinal) < json.IndexOf("2026-02-01", StringComparison.Ordinal));
        Assert.DoesNotContain("17:30", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Liability_builder_does_not_include_unapproved_fields()
    {
        var builder = new LiabilityAiContextBuilder();
        var json = builder.Build(new[] { new LiabilityAiContext("CreditCard", 100m, 24m) });

        Assert.Contains("outstandingAmount", json, StringComparison.Ordinal);
        Assert.Contains("interestRate", json, StringComparison.Ordinal);
        Assert.DoesNotContain("accountNumber", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("tenantId", json, StringComparison.OrdinalIgnoreCase);
    }
}
