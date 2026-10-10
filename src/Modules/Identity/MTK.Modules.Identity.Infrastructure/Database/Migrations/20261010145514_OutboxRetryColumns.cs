using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Identity.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class OutboxRetryColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Attempts",
                schema: "identity",
                table: "outbox_messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "NextAttemptOnUtc",
                schema: "identity",
                table: "outbox_messages",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attempts",
                schema: "identity",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "NextAttemptOnUtc",
                schema: "identity",
                table: "outbox_messages");
        }
    }
}
