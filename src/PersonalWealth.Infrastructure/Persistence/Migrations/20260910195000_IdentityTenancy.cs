using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalWealth.Infrastructure.Persistence.Migrations;

public partial class IdentityTenancy : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Tenants",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tenants", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserIdentities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserIdentities", x => x.Id);
                table.ForeignKey("FK_UserIdentities_Tenants_TenantId", x => x.TenantId, "Tenants", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_Tenants_Name", table: "Tenants", column: "Name", unique: true);
        migrationBuilder.CreateIndex(name: "IX_UserIdentities_TenantId_Subject", table: "UserIdentities", columns: new[] { "TenantId", "Subject" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_UserIdentities_TenantId_Email", table: "UserIdentities", columns: new[] { "TenantId", "Email" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "UserIdentities");
        migrationBuilder.DropTable(name: "Tenants");
    }
}
