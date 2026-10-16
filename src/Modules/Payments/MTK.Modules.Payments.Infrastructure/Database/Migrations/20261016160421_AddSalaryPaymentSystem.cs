using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSalaryPaymentSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SourceSalaryPaymentId",
                schema: "payments",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "salary_payments",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeFullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SalaryMonth = table.Column<int>(type: "integer", nullable: false),
                    SalaryYear = table.Column<int>(type: "integer", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalBonuses = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAdjustments = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    GrossSalary = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "EmployeeFullName", "Notes" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salary_payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salary_payment_lines",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SalaryPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    LineType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SourceBonusId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceAdjustmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salary_payment_lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_salary_payment_lines_salary_payments_SalaryPaymentId",
                        column: x => x.SalaryPaymentId,
                        principalSchema: "payments",
                        principalTable: "salary_payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SourceSalaryPaymentId",
                schema: "payments",
                table: "Transactions",
                column: "SourceSalaryPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_salary_payment_lines_SalaryPaymentId",
                schema: "payments",
                table: "salary_payment_lines",
                column: "SalaryPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_salary_payments_EmployeeId",
                schema: "payments",
                table: "salary_payments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_salary_payments_EmployeeId_SalaryYear_SalaryMonth",
                schema: "payments",
                table: "salary_payments",
                columns: new[] { "EmployeeId", "SalaryYear", "SalaryMonth" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_salary_payments_SalaryYear_SalaryMonth",
                schema: "payments",
                table: "salary_payments",
                columns: new[] { "SalaryYear", "SalaryMonth" });

            migrationBuilder.CreateIndex(
                name: "IX_salary_payments_SearchVector",
                schema: "payments",
                table: "salary_payments",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_salary_payments_Status",
                schema: "payments",
                table: "salary_payments",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_salary_payments_SourceSalaryPaymentId",
                schema: "payments",
                table: "Transactions",
                column: "SourceSalaryPaymentId",
                principalSchema: "payments",
                principalTable: "salary_payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_salary_payments_SourceSalaryPaymentId",
                schema: "payments",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "salary_payment_lines",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "salary_payments",
                schema: "payments");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SourceSalaryPaymentId",
                schema: "payments",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SourceSalaryPaymentId",
                schema: "payments",
                table: "Transactions");
        }
    }
}
