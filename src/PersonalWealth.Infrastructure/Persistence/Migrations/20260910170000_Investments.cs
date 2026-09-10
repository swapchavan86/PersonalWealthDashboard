using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalWealth.Infrastructure.Persistence.Migrations;

public partial class Investments : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "InvestmentAccounts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Institution = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                AccountNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                AccountType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InvestmentAccounts", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Securities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Symbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                SecurityType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                Isin = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Securities", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "InvestmentHoldings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                InvestmentAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SecurityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Quantity = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                CostBasis = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InvestmentHoldings", x => x.Id);
                table.ForeignKey("FK_InvestmentHoldings_InvestmentAccounts_InvestmentAccountId", x => x.InvestmentAccountId, "InvestmentAccounts", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_InvestmentHoldings_Securities_SecurityId", x => x.SecurityId, "Securities", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "InvestmentTransactions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                InvestmentAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SecurityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                TransactionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Quantity = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                Fees = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                Reference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InvestmentTransactions", x => x.Id);
                table.ForeignKey("FK_InvestmentTransactions_InvestmentAccounts_InvestmentAccountId", x => x.InvestmentAccountId, "InvestmentAccounts", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_InvestmentTransactions_Securities_SecurityId", x => x.SecurityId, "Securities", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_InvestmentAccounts_TenantId_AccountNumber",
            table: "InvestmentAccounts",
            columns: new[] { "TenantId", "AccountNumber" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Securities_TenantId_Symbol",
            table: "Securities",
            columns: new[] { "TenantId", "Symbol" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Securities_TenantId_Isin",
            table: "Securities",
            columns: new[] { "TenantId", "Isin" });

        migrationBuilder.CreateIndex(
            name: "IX_InvestmentHoldings_TenantId_InvestmentAccountId_SecurityId",
            table: "InvestmentHoldings",
            columns: new[] { "TenantId", "InvestmentAccountId", "SecurityId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_InvestmentTransactions_TenantId_InvestmentAccountId_TransactionDate",
            table: "InvestmentTransactions",
            columns: new[] { "TenantId", "InvestmentAccountId", "TransactionDate" });

        migrationBuilder.CreateIndex(
            name: "IX_InvestmentTransactions_TenantId_SecurityId_TransactionDate",
            table: "InvestmentTransactions",
            columns: new[] { "TenantId", "SecurityId", "TransactionDate" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "InvestmentTransactions");
        migrationBuilder.DropTable(name: "InvestmentHoldings");
        migrationBuilder.DropTable(name: "InvestmentAccounts");
        migrationBuilder.DropTable(name: "Securities");
    }
}
