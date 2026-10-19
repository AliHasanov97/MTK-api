using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Buildings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAssignedToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_appeals_AssignedToId",
                schema: "buildings",
                table: "appeals");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                schema: "buildings",
                table: "appeals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToId",
                schema: "buildings",
                table: "appeals",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_appeals_AssignedToId",
                schema: "buildings",
                table: "appeals",
                column: "AssignedToId");
        }
    }
}
