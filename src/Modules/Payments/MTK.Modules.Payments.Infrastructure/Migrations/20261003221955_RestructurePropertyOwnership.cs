using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestructurePropertyOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---- 1) New nullable columns, added alongside the old ones for backfill. ----

            migrationBuilder.AddColumn<Guid>(
                name: "ApartmentId", schema: "payments", table: "property_ownerships", type: "uuid", nullable: true);
            migrationBuilder.AddColumn<Guid>(
                name: "GarageId", schema: "payments", table: "property_ownerships", type: "uuid", nullable: true);

            // ---- 2) Backfill from the old PropertyType(int)+PropertyId pair.
            // PropertyType: Apartment = 0, Garage = 1 (see Charges/PropertyType.cs). ----

            migrationBuilder.Sql(
                "UPDATE payments.property_ownerships SET \"ApartmentId\" = \"PropertyId\" WHERE \"PropertyType\" = 0;");
            migrationBuilder.Sql(
                "UPDATE payments.property_ownerships SET \"GarageId\" = \"PropertyId\" WHERE \"PropertyType\" = 1;");

            // ---- 3) Drop old indexes, then old columns. ----

            migrationBuilder.DropIndex(
                name: "IX_property_ownerships_PropertyId", schema: "payments", table: "property_ownerships");
            migrationBuilder.DropIndex(
                name: "IX_property_ownerships_PropertyType", schema: "payments", table: "property_ownerships");

            migrationBuilder.DropColumn(name: "AreaSquareMeters", schema: "payments", table: "property_ownerships");
            migrationBuilder.DropColumn(name: "GarageType", schema: "payments", table: "property_ownerships");
            migrationBuilder.DropColumn(name: "PropertyId", schema: "payments", table: "property_ownerships");
            migrationBuilder.DropColumn(name: "PropertyNumber", schema: "payments", table: "property_ownerships");
            migrationBuilder.DropColumn(name: "PropertyType", schema: "payments", table: "property_ownerships");

            // ---- 4) New indexes + FK constraints. ----

            migrationBuilder.CreateIndex(
                name: "IX_property_ownerships_ApartmentId",
                schema: "payments",
                table: "property_ownerships",
                column: "ApartmentId",
                unique: true,
                filter: "\"ApartmentId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_property_ownerships_GarageId",
                schema: "payments",
                table: "property_ownerships",
                column: "GarageId",
                unique: true,
                filter: "\"GarageId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_property_ownerships_apartments_ApartmentId",
                schema: "payments",
                table: "property_ownerships",
                column: "ApartmentId",
                principalSchema: "payments",
                principalTable: "apartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_property_ownerships_garages_GarageId",
                schema: "payments",
                table: "property_ownerships",
                column: "GarageId",
                principalSchema: "payments",
                principalTable: "garages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_property_ownerships_owners_OwnerId",
                schema: "payments",
                table: "property_ownerships",
                column: "OwnerId",
                principalSchema: "payments",
                principalTable: "owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_property_ownerships_apartments_ApartmentId", schema: "payments", table: "property_ownerships");
            migrationBuilder.DropForeignKey(
                name: "FK_property_ownerships_garages_GarageId", schema: "payments", table: "property_ownerships");
            migrationBuilder.DropForeignKey(
                name: "FK_property_ownerships_owners_OwnerId", schema: "payments", table: "property_ownerships");

            migrationBuilder.DropIndex(
                name: "IX_property_ownerships_ApartmentId", schema: "payments", table: "property_ownerships");
            migrationBuilder.DropIndex(
                name: "IX_property_ownerships_GarageId", schema: "payments", table: "property_ownerships");

            migrationBuilder.AddColumn<decimal>(
                name: "AreaSquareMeters", schema: "payments", table: "property_ownerships",
                type: "numeric(18,2)", precision: 18, scale: 2, nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "GarageType", schema: "payments", table: "property_ownerships",
                type: "character varying(50)", maxLength: 50, nullable: true);
            migrationBuilder.AddColumn<Guid>(
                name: "PropertyId", schema: "payments", table: "property_ownerships", type: "uuid", nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "PropertyNumber", schema: "payments", table: "property_ownerships",
                type: "character varying(50)", maxLength: 50, nullable: true);
            migrationBuilder.AddColumn<int>(
                name: "PropertyType", schema: "payments", table: "property_ownerships", type: "integer", nullable: true);

            migrationBuilder.Sql(
                "UPDATE payments.property_ownerships SET \"PropertyType\" = 0, \"PropertyId\" = \"ApartmentId\" WHERE \"ApartmentId\" IS NOT NULL;");
            migrationBuilder.Sql(
                "UPDATE payments.property_ownerships SET \"PropertyType\" = 1, \"PropertyId\" = \"GarageId\" WHERE \"GarageId\" IS NOT NULL;");

            migrationBuilder.AlterColumn<Guid>(
                name: "PropertyId", schema: "payments", table: "property_ownerships", type: "uuid", nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
            migrationBuilder.AlterColumn<int>(
                name: "PropertyType", schema: "payments", table: "property_ownerships", type: "integer", nullable: false,
                defaultValue: 0);
            migrationBuilder.AlterColumn<decimal>(
                name: "AreaSquareMeters", schema: "payments", table: "property_ownerships",
                type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m);

            migrationBuilder.DropColumn(name: "ApartmentId", schema: "payments", table: "property_ownerships");
            migrationBuilder.DropColumn(name: "GarageId", schema: "payments", table: "property_ownerships");

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
    }
}
