using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalWealth.Infrastructure.Persistence.Migrations;

public partial class Expenses : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ExpenseCategories",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                ParentCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_ExpenseCategories", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Expenses",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ExpenseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                BankTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Expenses", x => x.Id));

        migrationBuilder.CreateTable(
            name: "RecurringExpenses",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Frequency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Interval = table.Column<int>(type: "int", nullable: false),
                EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_RecurringExpenses", x => x.Id));

        migrationBuilder.CreateIndex(name: "IX_ExpenseCategories_TenantId_Name", table: "ExpenseCategories", columns: new[] { "TenantId", "Name" });
        migrationBuilder.CreateIndex(name: "IX_ExpenseCategories_TenantId_ParentCategoryId", table: "ExpenseCategories", columns: new[] { "TenantId", "ParentCategoryId" });
        migrationBuilder.CreateIndex(name: "IX_Expenses_TenantId_ExpenseDate", table: "Expenses", columns: new[] { "TenantId", "ExpenseDate" });
        migrationBuilder.CreateIndex(name: "IX_Expenses_TenantId_CategoryId_ExpenseDate", table: "Expenses", columns: new[] { "TenantId", "CategoryId", "ExpenseDate" });
        migrationBuilder.CreateIndex(name: "IX_Expenses_TenantId_BankTransactionId", table: "Expenses", columns: new[] { "TenantId", "BankTransactionId" }, unique: true, filter: "[BankTransactionId] IS NOT NULL");
        migrationBuilder.CreateIndex(name: "IX_RecurringExpenses_TenantId_StartDate", table: "RecurringExpenses", columns: new[] { "TenantId", "StartDate" });
        migrationBuilder.CreateIndex(name: "IX_RecurringExpenses_TenantId_IsActive", table: "RecurringExpenses", columns: new[] { "TenantId", "IsActive" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "RecurringExpenses");
        migrationBuilder.DropTable(name: "Expenses");
        migrationBuilder.DropTable(name: "ExpenseCategories");
    }
}
