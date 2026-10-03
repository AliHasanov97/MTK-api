using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Buildings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityIdToUserSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityId",
                schema: "buildings",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdentityId",
                schema: "buildings",
                table: "Users",
                column: "IdentityId",
                filter: "\"IdentityId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_IdentityId",
                schema: "buildings",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IdentityId",
                schema: "buildings",
                table: "Users");
        }
    }
}
