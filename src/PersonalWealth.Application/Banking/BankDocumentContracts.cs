using PersonalWealth.Application.Documents.Tabular;

namespace PersonalWealth.Application.Banking;

public sealed record ParsedBankRow(int RowNumber, DateTime TransactionDate, decimal Amount, string Direction, string Description, decimal? Balance, string AccountNumber, string Fingerprint);
public sealed record BankParseError(int RowNumber, string Code, string Message);
public sealed record BankParseResult(IReadOnlyCollection<ParsedBankRow> Rows, IReadOnlyCollection<BankParseError> Errors, decimal? OpeningBalance, decimal? ClosingBalance);

public interface IBankDocumentParser
{
    BankParseResult Parse(string content, TabularTemplate template);
}
