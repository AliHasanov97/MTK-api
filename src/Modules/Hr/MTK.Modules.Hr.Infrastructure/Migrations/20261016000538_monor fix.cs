using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Hr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class monorfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalPaid",
                schema: "hr",
                table: "salary_deductions",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPaid",
                schema: "hr",
                table: "salary_deductions");
        }
    }
}
