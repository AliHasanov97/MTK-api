using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddChargeSnapshotFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AreaSquareMeters",
                schema: "payments",
                table: "Charges",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RateAmount",
                schema: "payments",
                table: "Charges",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RateType",
                schema: "payments",
                table: "Charges",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaSquareMeters",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropColumn(
                name: "RateAmount",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropColumn(
                name: "RateType",
                schema: "payments",
                table: "Charges");
        }
    }
}
