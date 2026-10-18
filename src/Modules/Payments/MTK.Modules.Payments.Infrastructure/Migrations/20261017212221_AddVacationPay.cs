using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVacationPay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vacation_pays",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VacationOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeFullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AverageMonthlyWage = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    AverageDailyWage = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    VacationDays = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    VacationStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    VacationEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaidAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacation_pays", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_vacation_pays_DueDate",
                schema: "payments",
                table: "vacation_pays",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_pays_EmployeeId",
                schema: "payments",
                table: "vacation_pays",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_pays_Status",
                schema: "payments",
                table: "vacation_pays",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_pays_Status_DueDate",
                schema: "payments",
                table: "vacation_pays",
                columns: new[] { "Status", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_vacation_pays_VacationOrderId",
                schema: "payments",
                table: "vacation_pays",
                column: "VacationOrderId",
                unique: true,
                filter: "\"DeletedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vacation_pays",
                schema: "payments");
        }
    }
}
