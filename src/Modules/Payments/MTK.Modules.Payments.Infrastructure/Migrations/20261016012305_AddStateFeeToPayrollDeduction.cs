using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStateFeeToPayrollDeduction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDeductionAmount",
                schema: "payments",
                table: "payroll_deductions");

            migrationBuilder.AddColumn<decimal>(
                name: "DebtRemaining",
                schema: "payments",
                table: "payroll_deductions",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "StateFee",
                schema: "payments",
                table: "payroll_deductions",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StateFeeRemaining",
                schema: "payments",
                table: "payroll_deductions",
                type: "numeric(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DebtRemaining",
                schema: "payments",
                table: "payroll_deductions");

            migrationBuilder.DropColumn(
                name: "StateFee",
                schema: "payments",
                table: "payroll_deductions");

            migrationBuilder.DropColumn(
                name: "StateFeeRemaining",
                schema: "payments",
                table: "payroll_deductions");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxDeductionAmount",
                schema: "payments",
                table: "payroll_deductions",
                type: "numeric",
                nullable: true);
        }
    }
}
