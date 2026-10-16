using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Hr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStateFeeToPayrollDeduction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPaid",
                schema: "hr",
                table: "salary_deductions",
                newName: "StateFeeRemaining");

            migrationBuilder.AddColumn<decimal>(
                name: "DebtRemaining",
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
                name: "DebtRemaining",
                schema: "hr",
                table: "salary_deductions");

            migrationBuilder.RenameColumn(
                name: "StateFeeRemaining",
                schema: "hr",
                table: "salary_deductions",
                newName: "TotalPaid");
        }
    }
}
