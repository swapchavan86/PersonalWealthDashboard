namespace PersonalWealth.Application.AI;

public sealed record AiInterpretationRequest(
    string ContextType,
    string Prompt,
    string ContextJson);

public sealed record AiInterpretationResponse(
    string Summary,
    IReadOnlyList<string> Insights,
    IReadOnlyList<string> Caveats);

public interface IAiProvider
{
    Task<AiInterpretationResponse> InterpretAsync(
        AiInterpretationRequest request,
        CancellationToken cancellationToken = default);
}

public interface IFinancialContextBuilder<in TInput>
{
    string Build(TInput input);
}

public sealed record WealthAiContext(
    decimal Cash,
    decimal Investments,
    decimal OtherAssets,
    decimal Liabilities,
    decimal NetWorth,
    decimal SavingsRate);

public sealed record InvestmentAiContext(
    string Symbol,
    decimal Quantity,
    decimal CostBasis,
    decimal CurrentValue);

public sealed record ExpenseAiContext(
    DateTime ExpenseDate,
    decimal Amount,
    string Category);

public sealed record LiabilityAiContext(
    string Type,
    decimal OutstandingAmount,
    decimal InterestRate);
