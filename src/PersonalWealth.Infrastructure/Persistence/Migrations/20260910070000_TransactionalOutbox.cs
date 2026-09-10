using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalWealth.Infrastructure.Persistence.Migrations;

public partial class TransactionalOutbox : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "OutboxMessages",
            columns: table => new
            {
                EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                EventType = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OccurredAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                PublishedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OutboxMessages", x => x.EventId);
            });

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessages_PublishedAtUtc_CreatedAtUtc",
            table: "OutboxMessages",
            columns: new[] { "PublishedAtUtc", "CreatedAtUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessages_TenantId_PublishedAtUtc_CreatedAtUtc",
            table: "OutboxMessages",
            columns: new[] { "TenantId", "PublishedAtUtc", "CreatedAtUtc" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "OutboxMessages");
    }
}
