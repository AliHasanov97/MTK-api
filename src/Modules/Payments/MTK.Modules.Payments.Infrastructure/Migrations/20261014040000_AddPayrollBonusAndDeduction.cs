using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollBonusAndDeduction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payroll_bonuses",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SalaryMonth = table.Column<int>(type: "integer", nullable: false),
                    SalaryYear = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_bonuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payroll_deductions",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PercentageSalary = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_deductions", x => x.Id);
                });

            migrationBuilder.AddColumn<decimal>(
                name: "BaseSalary",
                schema: "payments",
                table: "payslips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BonusAmount",
                schema: "payments",
                table: "payslips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CourtDeductionAmount",
                schema: "payments",
                table: "payslips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CourtDeductionPercentage",
                schema: "payments",
                table: "payslips",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalNetSalary",
                schema: "payments",
                table: "payslips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_payroll_bonuses_EmployeeId_SalaryYear_SalaryMonth",
                schema: "payments",
                table: "payroll_bonuses",
                columns: new[] { "EmployeeId", "SalaryYear", "SalaryMonth" });

            migrationBuilder.CreateIndex(
                name: "IX_payroll_bonuses_SalaryYear_SalaryMonth",
                schema: "payments",
                table: "payroll_bonuses",
                columns: new[] { "SalaryYear", "SalaryMonth" });

            migrationBuilder.CreateIndex(
                name: "IX_payroll_deductions_EmployeeId",
                schema: "payments",
                table: "payroll_deductions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_deductions_StartDate",
                schema: "payments",
                table: "payroll_deductions",
                column: "StartDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseSalary",
                schema: "payments",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "BonusAmount",
                schema: "payments",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "CourtDeductionAmount",
                schema: "payments",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "CourtDeductionPercentage",
                schema: "payments",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "FinalNetSalary",
                schema: "payments",
                table: "payslips");

            migrationBuilder.DropTable(
                name: "payroll_bonuses",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "payroll_deductions",
                schema: "payments");
        }
    }
}
