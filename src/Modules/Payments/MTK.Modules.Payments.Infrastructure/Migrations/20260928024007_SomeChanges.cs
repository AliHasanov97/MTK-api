using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SomeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "property_ownerships",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyType = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    AreaSquareMeters = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_ownerships", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_property_ownerships_OwnerId",
                schema: "payments",
                table: "property_ownerships",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_property_ownerships_PropertyId",
                schema: "payments",
                table: "property_ownerships",
                column: "PropertyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_property_ownerships_PropertyType",
                schema: "payments",
                table: "property_ownerships",
                column: "PropertyType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "property_ownerships",
                schema: "payments");
        }
    }
}
