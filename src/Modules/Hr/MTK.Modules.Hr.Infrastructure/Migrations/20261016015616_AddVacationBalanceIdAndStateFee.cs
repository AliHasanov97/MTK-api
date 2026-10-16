using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Hr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVacationBalanceIdAndStateFee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkYearEnd",
                schema: "hr",
                table: "vacation_compensation_applications");

            migrationBuilder.DropColumn(
                name: "WorkYearStart",
                schema: "hr",
                table: "vacation_compensation_applications");

            migrationBuilder.DropColumn(
                name: "WorkYearEnd",
                schema: "hr",
                table: "compensation_orders");

            migrationBuilder.DropColumn(
                name: "WorkYearStart",
                schema: "hr",
                table: "compensation_orders");

            migrationBuilder.AddColumn<Guid>(
                name: "VacationBalanceId",
                schema: "hr",
                table: "vacation_orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "VacationBalanceId",
                schema: "hr",
                table: "vacation_compensation_applications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "VacationBalanceId",
                schema: "hr",
                table: "vacation_applications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "VacationBalanceId",
                schema: "hr",
                table: "compensation_orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_vacation_orders_VacationBalanceId",
                schema: "hr",
                table: "vacation_orders",
                column: "VacationBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_compensation_applications_VacationBalanceId",
                schema: "hr",
                table: "vacation_compensation_applications",
                column: "VacationBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_vacation_applications_VacationBalanceId",
                schema: "hr",
                table: "vacation_applications",
                column: "VacationBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_compensation_orders_VacationBalanceId",
                schema: "hr",
                table: "compensation_orders",
                column: "VacationBalanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_compensation_orders_vacation_balances_VacationBalanceId",
                schema: "hr",
                table: "compensation_orders",
                column: "VacationBalanceId",
                principalSchema: "hr",
                principalTable: "vacation_balances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vacation_applications_vacation_balances_VacationBalanceId",
                schema: "hr",
                table: "vacation_applications",
                column: "VacationBalanceId",
                principalSchema: "hr",
                principalTable: "vacation_balances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vacation_compensation_applications_vacation_balances_Vacati~",
                schema: "hr",
                table: "vacation_compensation_applications",
                column: "VacationBalanceId",
                principalSchema: "hr",
                principalTable: "vacation_balances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vacation_orders_vacation_balances_VacationBalanceId",
                schema: "hr",
                table: "vacation_orders",
                column: "VacationBalanceId",
                principalSchema: "hr",
                principalTable: "vacation_balances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_compensation_orders_vacation_balances_VacationBalanceId",
                schema: "hr",
                table: "compensation_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_vacation_applications_vacation_balances_VacationBalanceId",
                schema: "hr",
                table: "vacation_applications");

            migrationBuilder.DropForeignKey(
                name: "FK_vacation_compensation_applications_vacation_balances_Vacati~",
                schema: "hr",
                table: "vacation_compensation_applications");

            migrationBuilder.DropForeignKey(
                name: "FK_vacation_orders_vacation_balances_VacationBalanceId",
                schema: "hr",
                table: "vacation_orders");

            migrationBuilder.DropIndex(
                name: "IX_vacation_orders_VacationBalanceId",
                schema: "hr",
                table: "vacation_orders");

            migrationBuilder.DropIndex(
                name: "IX_vacation_compensation_applications_VacationBalanceId",
                schema: "hr",
                table: "vacation_compensation_applications");

            migrationBuilder.DropIndex(
                name: "IX_vacation_applications_VacationBalanceId",
                schema: "hr",
                table: "vacation_applications");

            migrationBuilder.DropIndex(
                name: "IX_compensation_orders_VacationBalanceId",
                schema: "hr",
                table: "compensation_orders");

            migrationBuilder.DropColumn(
                name: "VacationBalanceId",
                schema: "hr",
                table: "vacation_orders");

            migrationBuilder.DropColumn(
                name: "VacationBalanceId",
                schema: "hr",
                table: "vacation_compensation_applications");

            migrationBuilder.DropColumn(
                name: "VacationBalanceId",
                schema: "hr",
                table: "vacation_applications");

            migrationBuilder.DropColumn(
                name: "VacationBalanceId",
                schema: "hr",
                table: "compensation_orders");

            migrationBuilder.AddColumn<DateOnly>(
                name: "WorkYearEnd",
                schema: "hr",
                table: "vacation_compensation_applications",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "WorkYearStart",
                schema: "hr",
                table: "vacation_compensation_applications",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "WorkYearEnd",
                schema: "hr",
                table: "compensation_orders",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "WorkYearStart",
                schema: "hr",
                table: "compensation_orders",
                type: "date",
                nullable: true);
        }
    }
}
