using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalWealth.Infrastructure.Persistence.Migrations;

public partial class IdentityTenancy : Migration
{
    protected override void Up(MigrationBuilder m)
    {
        m.CreateTable(
            "Tenants",
            t => new
            {
                Id = t.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = t.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                IsActive = t.Column<bool>(type: "bit", nullable: false),
                CreatedAt = t.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = t.Column<DateTime>(type: "datetime2", nullable: true)
            },
            c => c.PrimaryKey("PK_Tenants", x => x.Id));

        m.CreateTable(
            "UserIdentities",
            t => new
            {
                Id = t.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = t.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Subject = t.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Email = t.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                IsActive = t.Column<bool>(type: "bit", nullable: false),
                CreatedAt = t.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = t.Column<DateTime>(type: "datetime2", nullable: true)
            },
            c =>
            {
                c.PrimaryKey("PK_UserIdentities", x => x.Id);
                c.ForeignKey("FK_UserIdentities_Tenants_TenantId", x => x.TenantId, "Tenants", "Id", onDelete: ReferentialAction.Cascade);
            });

        m.CreateIndex("IX_Tenants_Name", "Tenants", "Name", unique: true);
        m.CreateIndex("IX_UserIdentities_TenantId_Subject", "UserIdentities", new[] { "TenantId", "Subject" }, unique: true);
        m.CreateIndex("IX_UserIdentities_TenantId_Email", "UserIdentities", new[] { "TenantId", "Email" });
    }

    protected override void Down(MigrationBuilder m)
    {
        m.DropTable("UserIdentities");
        m.DropTable("Tenants");
    }
}
