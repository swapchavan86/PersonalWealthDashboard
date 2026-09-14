using System.Text;
using Microsoft.EntityFrameworkCore;
using PersonalWealth.Application.Imports;
using PersonalWealth.Infrastructure.Documents;
using PersonalWealth.Infrastructure.Persistence;
using PersonalWealth.IntegrationTests.Persistence;
using Xunit;

namespace PersonalWealth.IntegrationTests.Imports;

public sealed class WealthWorkbookImportTests
{
    [Fact]
    public async Task Unified_template_populates_financial_records_and_is_idempotent()
    {
        await using var database = SqlServerTestDatabase.Create();
        await database.ResetAsync();
        var tenantId = Guid.NewGuid();
        var csv = "RecordType,ExternalId,Date,Institution,AccountNumber,AccountType,Currency,Description,Direction,Amount,Category,SecuritySymbol,SecurityName,SecurityType,Quantity,UnitPrice,Fees,AssetName,AssetType,AcquisitionValue,LiabilityName,LiabilityType,Principal,InterestRate,MaturityDate,PrincipalAmount,InterestAmount,ValuationValue\n" +
                  "BANK_ACCOUNT,bank-1,,HDFC,HDFC-001,Savings,INR,,,,,,,,,,,,,,,,,,,,,\n" +
                  "BANK_TRANSACTION,txn-1,2026-08-01,,HDFC-001,,INR,Salary,Credit,100000,,,,,,,,,,,,,,,,,,\n" +
                  "INVESTMENT_ACCOUNT,inv-1,,Zerodha,Z-001,Brokerage,INR,,,,,,,,,,,,,,,,,,,,,\n" +
                  "SECURITY,sec-1,,,,,INR,,, , ,RELIANCE,Reliance Industries,Equity,,,,,,,,,,,,,,\n" +
                  "INVESTMENT_TRANSACTION,itx-1,2026-08-02,,Z-001,,INR,BUY,,,,RELIANCE,,,10,2500,10,,,,,,,,,,,\n" +
                  "ASSET,asset-1,2024-01-01,,, ,INR,,,,,,,,,,,Home,Property,5000000,,,,,,,,\n" +
                  "ASSET_VALUATION,av-1,2026-08-31,,, ,INR,,,,,,,,,,,Home,Property,,,,,,,,,5500000\n" +
                  "LIABILITY,liab-1,2024-01-01,,, ,INR,,,,,,,,,,,,,,Home Loan,Mortgage,2000000,8.5,2029-01-01,,,\n";

        await using var context = CreateContext(database, tenantId);
        var service = new WealthWorkbookImportService(context, new TenantContext(tenantId));
        await using var first = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var result = await service.ImportAsync(first, "PersonalWealth_Import_Template.csv");

        Assert.True(result.Success, string.Join("; ", result.Errors));
        Assert.Equal(8, result.RowsImported);
        Assert.Equal(1, await context.BankAccounts.CountAsync());
        Assert.Equal(1, await context.BankTransactions.CountAsync());
        Assert.Equal(1, await context.InvestmentAccounts.CountAsync());
        Assert.Equal(1, await context.Securities.CountAsync());
        Assert.Equal(1, await context.InvestmentTransactions.CountAsync());
        Assert.Equal(1, await context.InvestmentHoldings.CountAsync());
        Assert.Equal(1, await context.Assets.CountAsync());
        Assert.Equal(1, await context.AssetValuations.CountAsync());
        Assert.Equal(1, await context.Liabilities.CountAsync());

        await using var second = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var secondResult = await service.ImportAsync(second, "PersonalWealth_Import_Template.csv");
        Assert.True(secondResult.Success, string.Join("; ", secondResult.Errors));
        Assert.Equal(0, secondResult.RowsImported);
        Assert.Equal(1, await context.BankTransactions.CountAsync());
        Assert.Equal(25010m, await context.InvestmentHoldings.Select(x => x.CostBasis).SingleAsync());
    }

    private static PersonalWealthDbContext CreateContext(SqlServerTestDatabase database, Guid tenantId)
    {
        var options = new DbContextOptionsBuilder<PersonalWealthDbContext>().UseSqlServer(database.ConnectionString).Options;
        return new PersonalWealthDbContext(options, new TenantContext(tenantId));
    }
}
