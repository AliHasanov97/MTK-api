using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Buildings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTriggeredByUserIdToOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TriggeredByUserId",
                schema: "buildings",
                table: "outbox_messages",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TriggeredByUserId",
                schema: "buildings",
                table: "outbox_messages");
        }
    }
}
