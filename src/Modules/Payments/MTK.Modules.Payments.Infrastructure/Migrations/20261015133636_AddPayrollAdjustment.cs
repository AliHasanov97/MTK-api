using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollAdjustment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AdjustmentAmount",
                schema: "payments",
                table: "payslips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "payments",
                table: "employees",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "payroll_adjustments",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Hours = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NormHours = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SalaryMonth = table.Column<int>(type: "integer", nullable: false),
                    SalaryYear = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_adjustments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payroll_adjustments_EmployeeId_SalaryYear_SalaryMonth",
                schema: "payments",
                table: "payroll_adjustments",
                columns: new[] { "EmployeeId", "SalaryYear", "SalaryMonth" });

            migrationBuilder.CreateIndex(
                name: "IX_payroll_adjustments_SalaryYear_SalaryMonth",
                schema: "payments",
                table: "payroll_adjustments",
                columns: new[] { "SalaryYear", "SalaryMonth" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payroll_adjustments",
                schema: "payments");

            migrationBuilder.DropColumn(
                name: "AdjustmentAmount",
                schema: "payments",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "payments",
                table: "employees");
        }
    }
}
