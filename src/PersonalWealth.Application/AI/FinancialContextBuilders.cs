using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PersonalWealth.Application.AI;

public sealed class WealthAiContextBuilder : IFinancialContextBuilder<WealthAiContext>
{
    public string Build(WealthAiContext input) => AiJson.Serialize(new
    {
        input.Cash,
        input.Investments,
        input.OtherAssets,
        input.Liabilities,
        input.NetWorth,
        input.SavingsRate
    });
}

public sealed class InvestmentAiContextBuilder : IFinancialContextBuilder<IReadOnlyList<InvestmentAiContext>>
{
    public string Build(IReadOnlyList<InvestmentAiContext> input)
    {
        var ordered = input
            .Select(x => new InvestmentAiContext(
                x.Symbol.Trim().ToUpperInvariant(),
                x.Quantity,
                x.CostBasis,
                x.CurrentValue))
            .OrderBy(x => x.Symbol, StringComparer.Ordinal)
            .ThenBy(x => x.Quantity)
            .Select(x => new
            {
                x.Symbol,
                x.Quantity,
                x.CostBasis,
                x.CurrentValue
            });

        return AiJson.Serialize(new { Investments = ordered });
    }
}

public sealed class ExpenseAiContextBuilder : IFinancialContextBuilder<IReadOnlyList<ExpenseAiContext>>
{
    public string Build(IReadOnlyList<ExpenseAiContext> input)
    {
        var ordered = input
            .OrderBy(x => x.ExpenseDate)
            .ThenBy(x => x.Category, StringComparer.OrdinalIgnoreCase)
            .ThenBy(x => x.Amount)
            .Select(x => new
            {
                Date = x.ExpenseDate.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                x.Amount,
                Category = x.Category.Trim()
            });

        return AiJson.Serialize(new { Expenses = ordered });
    }
}

public sealed class LiabilityAiContextBuilder : IFinancialContextBuilder<IReadOnlyList<LiabilityAiContext>>
{
    public string Build(IReadOnlyList<LiabilityAiContext> input)
    {
        var ordered = input
            .OrderBy(x => x.Type, StringComparer.OrdinalIgnoreCase)
            .ThenBy(x => x.OutstandingAmount)
            .Select(x => new
            {
                Type = x.Type.Trim(),
                x.OutstandingAmount,
                x.InterestRate
            });

        return AiJson.Serialize(new { Liabilities = ordered });
    }
}

internal static class AiJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        NumberHandling = JsonNumberHandling.Strict
    };

    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);
}
