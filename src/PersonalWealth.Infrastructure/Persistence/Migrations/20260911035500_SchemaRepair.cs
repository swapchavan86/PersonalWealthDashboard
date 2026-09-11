using System.IO.Compression;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalWealth.Infrastructure.Persistence.Migrations;

public partial class SchemaRepair : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(Decompress(@"
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"));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
DROP TABLE IF EXISTS [InvestmentTransactions];
DROP TABLE IF EXISTS [InvestmentHoldings];
DROP TABLE IF EXISTS [CorporateActions];
DROP TABLE IF EXISTS [LiabilityRepayments];
DROP TABLE IF EXISTS [BankTransactions];
DROP TABLE IF EXISTS [AssetValuations];
DROP TABLE IF EXISTS [Expenses];
DROP TABLE IF EXISTS [ExpenseCategories];
DROP TABLE IF EXISTS [ImportDuplicateDecisions];
DROP TABLE IF EXISTS [ImportIdentities];
DROP TABLE IF EXISTS [RecurringExpenses];
DROP TABLE IF EXISTS [UserIdentities];
DROP TABLE IF EXISTS [InvestmentAccounts];
DROP TABLE IF EXISTS [Securities];
DROP TABLE IF EXISTS [Liabilities];
DROP TABLE IF EXISTS [BankAccounts];
DROP TABLE IF EXISTS [Assets];
DROP TABLE IF EXISTS [Tenants];
DROP TABLE IF EXISTS [ProcessedEvents];
DROP TABLE IF EXISTS [OutboxMessages];
""");
    }

    private static string Decompress(string base64)
    {
        using var input = new MemoryStream(Convert.FromBase64String(base64));
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var reader = new StreamReader(gzip, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}
