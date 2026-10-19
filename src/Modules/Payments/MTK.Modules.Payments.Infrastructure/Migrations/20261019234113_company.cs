using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class company : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "payments",
                table: "companies",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "info@mtk.az");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                schema: "payments",
                table: "companies",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkHours",
                schema: "payments",
                table: "companies",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "payments",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "Phone",
                schema: "payments",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "WorkHours",
                schema: "payments",
                table: "companies");
        }
    }
}
