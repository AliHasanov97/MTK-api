using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Hr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVacationBalances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vacation_balances",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkYearStart = table.Column<DateOnly>(type: "date", nullable: false),
                    WorkYearEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    BaseEntitledDays = table.Column<int>(type: "integer", nullable: false),
                    ExperienceBonusDays = table.Column<int>(type: "integer", nullable: false),
                    KarabakhWorkerDays = table.Column<int>(type: "integer", nullable: false),
                    ChildrenUnder14Days = table.Column<int>(type: "integer", nullable: false),
                    DisabledChildDays = table.Column<int>(type: "integer", nullable: false),
                    CarriedOverDays = table.Column<int>(type: "integer", nullable: false),
                    UsedDays = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CompensatedDays = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacation_balances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vacation_balances_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_vacation_balances_EmployeeId_WorkYearStart",
                schema: "hr",
                table: "vacation_balances",
                columns: new[] { "EmployeeId", "WorkYearStart" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vacation_balances",
                schema: "hr");
        }
    }
}
