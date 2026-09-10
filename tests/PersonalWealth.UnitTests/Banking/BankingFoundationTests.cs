using PersonalWealth.Application.Banking;
using PersonalWealth.Application.Documents.Tabular;
using PersonalWealth.Infrastructure.Banking;
using Xunit;

namespace PersonalWealth.UnitTests.Banking;

public sealed class BankingFoundationTests
{
    [Fact]
    public void First_bank_template_declares_explicit_statement_semantics()
    {
        var fields = FirstBankStatementTemplate.Definition.Fields;
        Assert.Equal("sample-bank-csv", FirstBankStatementTemplate.TemplateId);
        Assert.Equal(1, FirstBankStatementTemplate.Version);
        Assert.Contains(fields, x => x.TargetField == "TransactionDate" && x.Type == TabularFieldType.Date);
        Assert.Contains(fields, x => x.AmountSemantic == AmountSemantic.Debit);
        Assert.Contains(fields, x => x.AmountSemantic == AmountSemantic.Credit);
    }

    [Fact]
    public void Parser_normalizes_rows_and_reports_malformed_rows()
    {
        var parser = new SampleBankCsvParser();
        var content = "Date,Description,Debit,Credit,Balance,AccountNumber\n2026-01-01,  Salary   Payment ,0,50000,50000,1234\nnot-a-date,Invalid,100,0,49900,1234\n2026-01-03,Utility,1000,0,48900,1234";
        var result = parser.Parse(content, FirstBankStatementTemplate.Definition);
        Assert.Equal(2, result.Rows.Count);
        Assert.Single(result.Errors);
        Assert.Equal("Salary Payment", result.Rows.First().Description);
        Assert.Equal("Credit", result.Rows.First().Direction);
        Assert.Equal(50000m, result.Rows.First().Amount);
        Assert.Equal("Debit", result.Rows.Last().Direction);
        Assert.Equal(1000m, result.Rows.Last().Amount);
    }

    [Fact]
    public void Categorizer_is_deterministic()
    {
        var categorizer = new RuleBasedTransactionCategorizer();
        var transaction = new BankTransactionModel(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Date, 100m, "Debit", "Grocery Supermarket", Guid.NewGuid(), "fp", null, null);
        Assert.Equal("Groceries", categorizer.Categorize(transaction));
        Assert.Equal("Groceries", categorizer.Categorize(transaction));
    }

    [Fact]
    public void Reconciler_detects_balanced_statement()
    {
        var reconciler = new StatementReconciler();
        var accountId = Guid.NewGuid();
        var transactions = new[]
        {
            new BankTransactionModel(Guid.NewGuid(), Guid.NewGuid(), accountId, DateTime.UtcNow.Date, 100m, "Credit", "Salary", Guid.NewGuid(), "a", null, null),
            new BankTransactionModel(Guid.NewGuid(), Guid.NewGuid(), accountId, DateTime.UtcNow.Date, 25m, "Debit", "Utility", Guid.NewGuid(), "b", null, null)
        };
        var result = reconciler.Reconcile(1000m, 1075m, transactions);
        Assert.True(result.IsMatch);
        Assert.Equal(0m, result.Difference);
    }

    [Fact]
    public void Transfer_detector_matches_owned_accounts_by_amount_and_date_window()
    {
        var detector = new TransferDetector();
        var tenant = Guid.NewGuid();
        var import = Guid.NewGuid();
        var date = new DateTime(2026, 1, 10);
        var debit = new BankTransactionModel(Guid.NewGuid(), tenant, Guid.NewGuid(), date, 1000m, "Debit", "Transfer", import, "d", null, null);
        var credit = new BankTransactionModel(Guid.NewGuid(), tenant, Guid.NewGuid(), date.AddDays(1), 1000m, "Credit", "Transfer", import, "c", null, null);
        var matches = detector.Detect([debit, credit]);
        Assert.Single(matches);
        Assert.Equal(1000m, matches.Single().Amount);
    }
}
