using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalWealth.Infrastructure.Persistence.Migrations;

public partial class AssetsLiabilities : Migration
{
    protected override void Up(MigrationBuilder m)
    {
        m.CreateTable(
            "Assets",
            t => new
            {
                Id = t.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = t.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = t.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Type = t.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                Currency = t.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                AcquisitionValue = t.Column<decimal>(type: "decimal(19,4)", nullable: false),
                AcquisitionDate = t.Column<DateTime>(type: "datetime2", nullable: false),
                IsActive = t.Column<bool>(type: "bit", nullable: false),
                CreatedAt = t.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = t.Column<DateTime>(type: "datetime2", nullable: true)
            },
            c => c.PrimaryKey("PK_Assets", x => x.Id));

        m.CreateTable(
            "Liabilities",
            t => new
            {
                Id = t.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = t.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = t.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Type = t.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                Currency = t.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                OriginalPrincipal = t.Column<decimal>(type: "decimal(19,4)", nullable: false),
                OutstandingPrincipal = t.Column<decimal>(type: "decimal(19,4)", nullable: false),
                AnnualInterestRate = t.Column<decimal>(type: "decimal(9,4)", nullable: false),
                StartDate = t.Column<DateTime>(type: "datetime2", nullable: false),
                MaturityDate = t.Column<DateTime>(type: "datetime2", nullable: true),
                IsActive = t.Column<bool>(type: "bit", nullable: false),
                CreatedAt = t.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = t.Column<DateTime>(type: "datetime2", nullable: true)
            },
            c => c.PrimaryKey("PK_Liabilities", x => x.Id));

        m.CreateTable(
            "AssetValuations",
            t => new
            {
                Id = t.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = t.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AssetId = t.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ValuationDate = t.Column<DateTime>(type: "datetime2", nullable: false),
                Value = t.Column<decimal>(type: "decimal(19,4)", nullable: false),
                Source = t.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                CreatedAt = t.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = t.Column<DateTime>(type: "datetime2", nullable: true)
            },
            c =>
            {
                c.PrimaryKey("PK_AssetValuations", x => x.Id);
                c.ForeignKey("FK_AssetValuations_Assets_AssetId", x => x.AssetId, "Assets", "Id", onDelete: ReferentialAction.Cascade);
            });

        m.CreateTable(
            "LiabilityRepayments",
            t => new
            {
                Id = t.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = t.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LiabilityId = t.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PaymentDate = t.Column<DateTime>(type: "datetime2", nullable: false),
                Amount = t.Column<decimal>(type: "decimal(19,4)", nullable: false),
                PrincipalAmount = t.Column<decimal>(type: "decimal(19,4)", nullable: false),
                InterestAmount = t.Column<decimal>(type: "decimal(19,4)", nullable: false),
                CreatedAt = t.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = t.Column<DateTime>(type: "datetime2", nullable: true)
            },
            c =>
            {
                c.PrimaryKey("PK_LiabilityRepayments", x => x.Id);
                c.ForeignKey("FK_LiabilityRepayments_Liabilities_LiabilityId", x => x.LiabilityId, "Liabilities", "Id", onDelete: ReferentialAction.Cascade);
            });

        m.CreateIndex("IX_Assets_TenantId_Name", "Assets", new[] { "TenantId", "Name" });
        m.CreateIndex("IX_Liabilities_TenantId_Name", "Liabilities", new[] { "TenantId", "Name" });
        m.CreateIndex("IX_AssetValuations_TenantId_AssetId_ValuationDate", "AssetValuations", new[] { "TenantId", "AssetId", "ValuationDate" });
        m.CreateIndex("IX_LiabilityRepayments_TenantId_LiabilityId_PaymentDate", "LiabilityRepayments", new[] { "TenantId", "LiabilityId", "PaymentDate" });
    }

    protected override void Down(MigrationBuilder m)
    {
        m.DropTable("LiabilityRepayments");
        m.DropTable("AssetValuations");
        m.DropTable("Liabilities");
        m.DropTable("Assets");
    }
}
