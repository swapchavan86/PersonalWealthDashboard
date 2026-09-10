namespace PersonalWealth.Application.Investments;

public sealed record InvestmentImportRow(int RowNumber, string AccountNumber, string Symbol, DateTime TransactionDate, string TransactionType, decimal Quantity, decimal UnitPrice, decimal Fees, string Currency, string? Reference);
public sealed record InvestmentImportResult(IReadOnlyList<InvestmentImportRow> Accepted, IReadOnlyList<(int RowNumber, string Error)> Rejected);

public interface IInvestmentImportService
{
    InvestmentImportResult Validate(IEnumerable<InvestmentImportRow> rows);
}

public sealed class InvestmentImportService : IInvestmentImportService
{
    private static readonly HashSet<string> SupportedTypes = new(StringComparer.OrdinalIgnoreCase) { "BUY", "SELL", "DIVIDEND", "FEE" };

    public InvestmentImportResult Validate(IEnumerable<InvestmentImportRow> rows)
    {
        var accepted = new List<InvestmentImportRow>();
        var rejected = new List<(int, string)>();
        foreach (var row in rows.OrderBy(x => x.RowNumber))
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(row.AccountNumber)) errors.Add("Account number is required.");
            if (string.IsNullOrWhiteSpace(row.Symbol)) errors.Add("Symbol is required.");
            if (row.TransactionDate == default) errors.Add("Transaction date is required.");
            if (!SupportedTypes.Contains(row.TransactionType)) errors.Add("Unsupported transaction type.");
            if (row.Quantity < 0) errors.Add("Quantity cannot be negative.");
            if (row.UnitPrice < 0) errors.Add("Unit price cannot be negative.");
            if (row.Fees < 0) errors.Add("Fees cannot be negative.");
            if (string.IsNullOrWhiteSpace(row.Currency)) errors.Add("Currency is required.");
            if ((row.TransactionType.Equals("BUY", StringComparison.OrdinalIgnoreCase) || row.TransactionType.Equals("SELL", StringComparison.OrdinalIgnoreCase)) && row.Quantity <= 0) errors.Add("Buy/sell quantity must be positive.");
            if (errors.Count == 0) accepted.Add(row);
            else rejected.Add((row.RowNumber, string.Join(" ", errors)));
        }
        return new InvestmentImportResult(accepted, rejected);
    }
}
