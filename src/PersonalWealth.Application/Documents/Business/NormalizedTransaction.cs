namespace PersonalWealth.Application.Documents.Business;

public sealed record NormalizedTransaction(
    DateTime TransactionDate,
    decimal Amount,
    string Description,
    string? Currency = null,
    string? ExternalReference = null);
