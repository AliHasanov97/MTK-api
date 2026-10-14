using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Buildings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceFloorFieldsWithApartmentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApartmentsPerFloor",
                schema: "buildings",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "TotalFloors",
                schema: "buildings",
                table: "Buildings");

            migrationBuilder.AddColumn<Guid>(
                name: "ApartmentTypeId",
                schema: "buildings",
                table: "Apartments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApartmentTypes",
                schema: "buildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomCount = table.Column<int>(type: "integer", nullable: false),
                    AreaSquareMeters = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmentTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApartmentTypes_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalSchema: "buildings",
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_ApartmentTypeId",
                schema: "buildings",
                table: "Apartments",
                column: "ApartmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentTypes_BuildingId",
                schema: "buildings",
                table: "ApartmentTypes",
                column: "BuildingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_ApartmentTypes_ApartmentTypeId",
                schema: "buildings",
                table: "Apartments",
                column: "ApartmentTypeId",
                principalSchema: "buildings",
                principalTable: "ApartmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_ApartmentTypes_ApartmentTypeId",
                schema: "buildings",
                table: "Apartments");

            migrationBuilder.DropTable(
                name: "ApartmentTypes",
                schema: "buildings");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_ApartmentTypeId",
                schema: "buildings",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "ApartmentTypeId",
                schema: "buildings",
                table: "Apartments");

            migrationBuilder.AddColumn<int>(
                name: "ApartmentsPerFloor",
                schema: "buildings",
                table: "Buildings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalFloors",
                schema: "buildings",
                table: "Buildings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
