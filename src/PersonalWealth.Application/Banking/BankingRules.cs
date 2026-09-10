namespace PersonalWealth.Application.Banking;

public interface ITransactionNormalizer
{
    BankTransactionModel Normalize(BankTransactionModel transaction);
}

public interface ITransactionCategorizer
{
    string? Categorize(BankTransactionModel transaction);
}

public sealed class TransactionNormalizer : ITransactionNormalizer
{
    public BankTransactionModel Normalize(BankTransactionModel transaction) => transaction with
    {
        Description = string.Join(' ', transaction.Description.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)),
        Direction = transaction.Direction.Trim(),
        Category = string.IsNullOrWhiteSpace(transaction.Category) ? null : transaction.Category.Trim()
    };
}

public sealed class RuleBasedTransactionCategorizer : ITransactionCategorizer
{
    public string? Categorize(BankTransactionModel transaction)
    {
        var description = transaction.Description.ToLowerInvariant();
        if (description.Contains("salary") || description.Contains("payroll")) return "Income";
        if (description.Contains("rent") || description.Contains("electric") || description.Contains("utility")) return "Housing & Utilities";
        if (description.Contains("grocery") || description.Contains("supermarket")) return "Groceries";
        if (description.Contains("atm") || description.Contains("cash")) return "Cash";
        return transaction.Category;
    }
}
