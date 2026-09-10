using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalWealth.Infrastructure.Persistence.Migrations;

public partial class EventReliability : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "AttemptCount",
            table: "OutboxMessages",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "LastError",
            table: "OutboxMessages",
            type: "nvarchar(4000)",
            maxLength: 4000,
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "NextAttemptAtUtc",
            table: "OutboxMessages",
            type: "datetimeoffset",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "ProcessedEvents",
            columns: table => new
            {
                EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EventType = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                ProcessedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProcessedEvents", x => x.EventId);
            });

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessages_PublishedAtUtc_NextAttemptAtUtc_CreatedAtUtc",
            table: "OutboxMessages",
            columns: new[] { "PublishedAtUtc", "NextAttemptAtUtc", "CreatedAtUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessages_TenantId_PublishedAtUtc_NextAttemptAtUtc_CreatedAtUtc",
            table: "OutboxMessages",
            columns: new[] { "TenantId", "PublishedAtUtc", "NextAttemptAtUtc", "CreatedAtUtc" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ProcessedEvents");
        migrationBuilder.DropIndex(
            name: "IX_OutboxMessages_PublishedAtUtc_NextAttemptAtUtc_CreatedAtUtc",
            table: "OutboxMessages");
        migrationBuilder.DropIndex(
            name: "IX_OutboxMessages_TenantId_PublishedAtUtc_NextAttemptAtUtc_CreatedAtUtc",
            table: "OutboxMessages");
        migrationBuilder.DropColumn(name: "AttemptCount", table: "OutboxMessages");
        migrationBuilder.DropColumn(name: "LastError", table: "OutboxMessages");
        migrationBuilder.DropColumn(name: "NextAttemptAtUtc", table: "OutboxMessages");
    }
}
