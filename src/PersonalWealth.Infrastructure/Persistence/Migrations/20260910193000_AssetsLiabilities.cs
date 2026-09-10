using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalWealth.Infrastructure.Persistence.Migrations;

public partial class AssetsLiabilities : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Assets",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                AcquisitionValue = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                AcquisitionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Assets", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Liabilities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                OriginalPrincipal = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                OutstandingPrincipal = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                AnnualInterestRate = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                MaturityDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Liabilities", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AssetValuations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ValuationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Value = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssetValuations", x => x.Id);
                table.ForeignKey("FK_AssetValuations_Assets_AssetId", x => x.AssetId, "Assets", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "LiabilityRepayments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LiabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                PrincipalAmount = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                InterestAmount = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LiabilityRepayments", x => x.Id);
                table.ForeignKey("FK_LiabilityRepayments_Liabilities_LiabilityId", x => x.LiabilityId, "Liabilities", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_Assets_TenantId_Name", table: "Assets", columns: new[] { "TenantId", "Name" });
        migrationBuilder.CreateIndex(name: "IX_Liabilities_TenantId_Name", table: "Liabilities", columns: new[] { "TenantId", "Name" });
        migrationBuilder.CreateIndex(name: "IX_AssetValuations_TenantId_AssetId_ValuationDate", table: "AssetValuations", columns: new[] { "TenantId", "AssetId", "ValuationDate" });
        migrationBuilder.CreateIndex(name: "IX_LiabilityRepayments_TenantId_LiabilityId_PaymentDate", table: "LiabilityRepayments", columns: new[] { "TenantId", "LiabilityId", "PaymentDate" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "LiabilityRepayments");
        migrationBuilder.DropTable(name: "AssetValuations");
        migrationBuilder.DropTable(name: "Liabilities");
        migrationBuilder.DropTable(name: "Assets");
    }
}
