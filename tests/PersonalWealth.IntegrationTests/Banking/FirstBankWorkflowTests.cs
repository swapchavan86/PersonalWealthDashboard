using Microsoft.EntityFrameworkCore;
using PersonalWealth.Application.Banking;
using PersonalWealth.Application.Documents.Business;
using PersonalWealth.Application.Documents.Tabular;
using PersonalWealth.Infrastructure.Banking;
using PersonalWealth.Infrastructure.Documents;
using PersonalWealth.Infrastructure.Persistence;
using PersonalWealth.Infrastructure.Persistence.Outbox;
using PersonalWealth.IntegrationTests.Persistence;
using PersonalWealth.Domain.Banking;
using Xunit;

namespace PersonalWealth.IntegrationTests.Banking;

public sealed class FirstBankWorkflowTests
{
    [Fact]
    public async Task Sample_statement_flows_from_parse_to_canonical_data_and_outbox()
    {
        await using var database = SqlServerTestDatabase.Create();
        await database.ResetAsync();
        var tenantId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        await using (var setup = CreateContext(database, tenantId))
        {
            setup.BankAccounts.Add(new BankAccount(accountId, tenantId, "Sample Bank", "1234", BankAccountType.Savings, "INR"));
            await setup.SaveChangesAsync();
        }

        const string csv = "Date,Description,Debit,Credit,Balance,AccountNumber\n2026-01-01,Salary Payment,0,50000,50000,1234\n2026-01-02,Grocery Supermarket,5000,0,45000,1234";
        await using var context = CreateContext(database, tenantId);
        var workflow = new FirstBankImportWorkflow(
            new DocumentTemplateSelector(),
            new SampleBankCsvParser(),
            new Infrastructure.Documents.Business.TransactionBusinessValidator(),
            new EfBankImportService(context, new TransactionNormalizer(), new RuleBasedTransactionCategorizer(), new StatementReconciler(), new EfCoreOutbox(context)));

        var result = await workflow.ImportAsync(new FirstBankWorkflowRequest(tenantId, accountId, Guid.NewGuid(), "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc", ".csv", csv, 0m, 45000m));

        Assert.Empty(result.Errors);
        Assert.Equal(2, result.ImportedCount);
        Assert.True(result.Reconciled);
        Assert.Equal(2, await context.BankTransactions.CountAsync());
        Assert.Single(await context.OutboxMessages.ToListAsync());
    }

    private static PersonalWealthDbContext CreateContext(SqlServerTestDatabase database, Guid tenantId)
    {
        var options = new DbContextOptionsBuilder<PersonalWealthDbContext>().UseSqlServer(database.ConnectionString).Options;
        return new PersonalWealthDbContext(options, new TenantContext(tenantId));
    }
}
