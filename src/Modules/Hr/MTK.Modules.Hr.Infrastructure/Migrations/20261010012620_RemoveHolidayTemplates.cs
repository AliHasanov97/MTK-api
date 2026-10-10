using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace MTK.Modules.Hr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHolidayTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_calendar_days_holiday_templates_HolidayTemplateId",
                schema: "hr",
                table: "calendar_days");

            migrationBuilder.DropTable(
                name: "holiday_templates",
                schema: "hr");

            migrationBuilder.DropIndex(
                name: "IX_calendar_days_HolidayTemplateId",
                schema: "hr",
                table: "calendar_days");

            migrationBuilder.DropColumn(
                name: "HolidayTemplateId",
                schema: "hr",
                table: "calendar_days");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HolidayTemplateId",
                schema: "hr",
                table: "calendar_days",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "holiday_templates",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    DaysCount = table.Column<int>(type: "integer", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('simple', coalesce(\"Name\",''))", stored: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_holiday_templates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_calendar_days_HolidayTemplateId",
                schema: "hr",
                table: "calendar_days",
                column: "HolidayTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_holiday_templates_Day_Month",
                schema: "hr",
                table: "holiday_templates",
                columns: new[] { "Day", "Month" });

            migrationBuilder.CreateIndex(
                name: "IX_holiday_templates_DeletedAt",
                schema: "hr",
                table: "holiday_templates",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_holiday_templates_Name",
                schema: "hr",
                table: "holiday_templates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_holiday_templates_SearchVector",
                schema: "hr",
                table: "holiday_templates",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.AddForeignKey(
                name: "FK_calendar_days_holiday_templates_HolidayTemplateId",
                schema: "hr",
                table: "calendar_days",
                column: "HolidayTemplateId",
                principalSchema: "hr",
                principalTable: "holiday_templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
