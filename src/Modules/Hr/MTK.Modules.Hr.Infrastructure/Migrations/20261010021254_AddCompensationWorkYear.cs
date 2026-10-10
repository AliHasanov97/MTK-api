using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Hr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompensationWorkYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
