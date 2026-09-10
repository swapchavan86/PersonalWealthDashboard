namespace PersonalWealth.Application.Banking;

public interface IStatementReconciler
{
    StatementReconciliationResult Reconcile(
        decimal openingBalance,
        decimal closingBalance,
        IReadOnlyCollection<BankTransactionModel> transactions);
}

public sealed record StatementReconciliationResult(
    bool IsMatch,
    decimal ExpectedClosingBalance,
    decimal ActualClosingBalance,
    decimal Difference);
