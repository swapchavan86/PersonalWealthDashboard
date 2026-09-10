using PersonalWealth.Application.Documents.Tabular;

namespace PersonalWealth.Application.Banking;

public static class FirstBankStatementTemplate
{
    public const string TemplateId = "sample-bank-csv";
    public const int Version = 1;

    public static TabularTemplate Definition { get; } = new(
        TemplateId,
        Version,
        new[]
        {
            new TabularField("TransactionDate", "Date", true, TabularFieldType.Date, "yyyy-MM-dd", "trim"),
            new TabularField("Description", "Description", true, TabularFieldType.Text, null, "collapse-whitespace"),
            new TabularField("Debit", "Debit", false, TabularFieldType.Decimal, null, "trim", AmountSemantic.Debit),
            new TabularField("Credit", "Credit", false, TabularFieldType.Decimal, null, "trim", AmountSemantic.Credit),
            new TabularField("Balance", "Balance", false, TabularFieldType.Decimal, null, "trim"),
            new TabularField("AccountNumber", "AccountNumber", true, TabularFieldType.Text, null, "trim")
        });
}
