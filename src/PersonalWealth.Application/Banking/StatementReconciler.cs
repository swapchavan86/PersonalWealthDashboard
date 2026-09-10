namespace PersonalWealth.Application.Banking;

public sealed class StatementReconciler : IStatementReconciler
{
    private const decimal Tolerance = 0.01m;

    public StatementReconciliationResult Reconcile(
        decimal openingBalance,
        decimal closingBalance,
        IReadOnlyCollection<BankTransactionModel> transactions)
    {
        var netMovement = transactions.Sum(transaction =>
            string.Equals(transaction.Direction, "Credit", StringComparison.OrdinalIgnoreCase)
                ? transaction.Amount
                : -transaction.Amount);

        var expectedClosingBalance = openingBalance + netMovement;
        var difference = closingBalance - expectedClosingBalance;

        return new(
            Math.Abs(difference) <= Tolerance,
            expectedClosingBalance,
            closingBalance,
            difference);
    }
}
