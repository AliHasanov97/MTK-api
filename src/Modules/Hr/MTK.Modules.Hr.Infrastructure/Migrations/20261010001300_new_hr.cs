using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Hr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class new_hr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_vacation_plans_CalendarYear",
                schema: "hr",
                table: "vacation_plans");

            migrationBuilder.DropIndex(
                name: "IX_vacation_plans_EmployeeId_CalendarYear",
                schema: "hr",
                table: "vacation_plans");

            migrationBuilder.DropIndex(
                name: "IX_vacation_plans_StartDate",
                schema: "hr",
                table: "vacation_plans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_vacation_plans_CalendarYear",
                schema: "hr",
                table: "vacation_plans",
                column: "CalendarYear");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_plans_EmployeeId_CalendarYear",
                schema: "hr",
                table: "vacation_plans",
                columns: new[] { "EmployeeId", "CalendarYear" });

            migrationBuilder.CreateIndex(
                name: "IX_vacation_plans_StartDate",
                schema: "hr",
                table: "vacation_plans",
                column: "StartDate");
        }
    }
}
