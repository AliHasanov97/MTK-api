using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Buildings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeOwnerUserIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Owners_UserId",
                schema: "buildings",
                table: "Owners");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "buildings",
                table: "Owners",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_Owners_UserId",
                schema: "buildings",
                table: "Owners",
                column: "UserId",
                unique: true,
                filter: "\"UserId\" IS NOT NULL AND \"DeletedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Owners_UserId",
                schema: "buildings",
                table: "Owners");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "buildings",
                table: "Owners",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Owners_UserId",
                schema: "buildings",
                table: "Owners",
                column: "UserId",
                unique: true,
                filter: "\"DeletedAt\" IS NULL");
        }
    }
}
