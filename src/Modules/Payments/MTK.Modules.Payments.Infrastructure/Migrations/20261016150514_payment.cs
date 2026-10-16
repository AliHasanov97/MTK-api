using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class payment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payroll_tax_brackets",
                schema: "payments");

            migrationBuilder.DropColumn(
                name: "EmployeeDsmf",
                schema: "payments",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "EmployeeMedicalInsurance",
                schema: "payments",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "EmployeeUnemployment",
                schema: "payments",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "EmployerDsmf",
                schema: "payments",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "EmployerMedicalInsurance",
                schema: "payments",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "EmployerUnemployment",
                schema: "payments",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "IncomeTax",
                schema: "payments",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "TaxableIncome",
                schema: "payments",
                table: "payslips");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EmployeeDsmf",
                schema: "payments",
                table: "payslips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EmployeeMedicalInsurance",
                schema: "payments",
                table: "payslips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EmployeeUnemployment",
                schema: "payments",
                table: "payslips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EmployerDsmf",
                schema: "payments",
                table: "payslips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EmployerMedicalInsurance",
                schema: "payments",
                table: "payslips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EmployerUnemployment",
                schema: "payments",
                table: "payslips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IncomeTax",
                schema: "payments",
                table: "payslips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxableIncome",
                schema: "payments",
                table: "payslips",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "payroll_tax_brackets",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EffectiveFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LowerBound = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Party = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    RateType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpperBound = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_tax_brackets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payroll_tax_brackets_EffectiveTo",
                schema: "payments",
                table: "payroll_tax_brackets",
                column: "EffectiveTo");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_tax_brackets_RateType_Party_EffectiveFrom",
                schema: "payments",
                table: "payroll_tax_brackets",
                columns: new[] { "RateType", "Party", "EffectiveFrom" });
        }
    }
}
