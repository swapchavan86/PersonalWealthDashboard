using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalWealth.Infrastructure.Persistence.Migrations;

public partial class Phase6CorporateActions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(name: "CorporateActions", columns: table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SecurityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ActionType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
            EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            RatioNumerator = table.Column<decimal>(type: "decimal(19,10)", nullable: false),
            RatioDenominator = table.Column<decimal>(type: "decimal(19,10)", nullable: false),
            NewSymbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            NewName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
        }, constraints: table => { table.PrimaryKey("PK_CorporateActions", x => x.Id); table.ForeignKey("FK_CorporateActions_Securities_SecurityId", x => x.SecurityId, "Securities", "Id", onDelete: ReferentialAction.Restrict); });
        migrationBuilder.CreateIndex(name: "IX_CorporateActions_TenantId_SecurityId_EffectiveDate", table: "CorporateActions", columns: new[] { "TenantId", "SecurityId", "EffectiveDate" });
    }
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "CorporateActions");
}
