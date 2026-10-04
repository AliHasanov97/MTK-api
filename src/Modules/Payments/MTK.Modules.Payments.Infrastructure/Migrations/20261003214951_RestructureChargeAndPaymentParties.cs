using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestructureChargeAndPaymentParties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---- 1) New nullable columns, added alongside the old ones so data can
            // still be read from PartyType/PartyId/PropertyType/PropertyId for backfill. ----

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId", schema: "payments", table: "Payments", type: "uuid", nullable: true);
            migrationBuilder.AddColumn<Guid>(
                name: "VendorId", schema: "payments", table: "Payments", type: "uuid", nullable: true);
            migrationBuilder.AddColumn<Guid>(
                name: "ApartmentId", schema: "payments", table: "Payments", type: "uuid", nullable: true);
            migrationBuilder.AddColumn<Guid>(
                name: "GarageId", schema: "payments", table: "Payments", type: "uuid", nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId", schema: "payments", table: "Charges", type: "uuid", nullable: true);
            migrationBuilder.AddColumn<Guid>(
                name: "VendorId", schema: "payments", table: "Charges", type: "uuid", nullable: true);
            migrationBuilder.AddColumn<Guid>(
                name: "ApartmentId", schema: "payments", table: "Charges", type: "uuid", nullable: true);
            migrationBuilder.AddColumn<Guid>(
                name: "GarageId", schema: "payments", table: "Charges", type: "uuid", nullable: true);

            // ---- 2) Backfill from the old discriminator+generic-id columns. Note:
            // EF's auto-scaffolding would have RENAMED PropertyId -> VendorId here (a
            // same-shape-column heuristic) — that would have been silently wrong, since
            // PropertyId holds an apartment/garage id, never a vendor id. Explicit SQL
            // instead. ----

            migrationBuilder.Sql(
                "UPDATE payments.\"Payments\" SET \"OwnerId\" = \"PartyId\" WHERE \"PartyType\" = 'Owner';");
            migrationBuilder.Sql(
                "UPDATE payments.\"Payments\" SET \"VendorId\" = \"PartyId\" WHERE \"PartyType\" = 'Vendor';");
            migrationBuilder.Sql(
                "UPDATE payments.\"Payments\" SET \"ApartmentId\" = \"PropertyId\" WHERE \"PropertyType\" = 'Apartment';");
            migrationBuilder.Sql(
                "UPDATE payments.\"Payments\" SET \"GarageId\" = \"PropertyId\" WHERE \"PropertyType\" = 'Garage';");

            migrationBuilder.Sql(
                "UPDATE payments.\"Charges\" SET \"OwnerId\" = \"PartyId\" WHERE \"PartyType\" = 'Owner';");
            migrationBuilder.Sql(
                "UPDATE payments.\"Charges\" SET \"VendorId\" = \"PartyId\" WHERE \"PartyType\" = 'Vendor';");
            migrationBuilder.Sql(
                "UPDATE payments.\"Charges\" SET \"ApartmentId\" = \"PropertyId\" WHERE \"PropertyType\" = 'Apartment';");
            migrationBuilder.Sql(
                "UPDATE payments.\"Charges\" SET \"GarageId\" = \"PropertyId\" WHERE \"PropertyType\" = 'Garage';");

            // ---- 3) Drop old indexes, then old columns. ----

            migrationBuilder.DropIndex(name: "IX_Payments_PartyId", schema: "payments", table: "Payments");
            migrationBuilder.DropIndex(name: "IX_Payments_PartyType", schema: "payments", table: "Payments");
            migrationBuilder.DropIndex(name: "IX_Payments_PropertyId", schema: "payments", table: "Payments");

            migrationBuilder.DropIndex(name: "IX_Charges_PartyId", schema: "payments", table: "Charges");
            migrationBuilder.DropIndex(name: "IX_Charges_PartyId_PropertyId_Period", schema: "payments", table: "Charges");
            migrationBuilder.DropIndex(name: "IX_Charges_PartyType", schema: "payments", table: "Charges");
            migrationBuilder.DropIndex(name: "IX_Charges_PartyType_PartyId_IssuedOn", schema: "payments", table: "Charges");
            migrationBuilder.DropIndex(name: "IX_Charges_PropertyId", schema: "payments", table: "Charges");

            migrationBuilder.DropColumn(name: "PartyId", schema: "payments", table: "Payments");
            migrationBuilder.DropColumn(name: "PartyType", schema: "payments", table: "Payments");
            migrationBuilder.DropColumn(name: "PropertyId", schema: "payments", table: "Payments");
            migrationBuilder.DropColumn(name: "PropertyType", schema: "payments", table: "Payments");

            migrationBuilder.DropColumn(name: "PartyId", schema: "payments", table: "Charges");
            migrationBuilder.DropColumn(name: "PartyType", schema: "payments", table: "Charges");
            migrationBuilder.DropColumn(name: "PropertyId", schema: "payments", table: "Charges");
            migrationBuilder.DropColumn(name: "PropertyType", schema: "payments", table: "Charges");

            // ---- 4) New indexes. ----

            migrationBuilder.CreateIndex(name: "IX_Payments_OwnerId", schema: "payments", table: "Payments", column: "OwnerId");
            migrationBuilder.CreateIndex(name: "IX_Payments_VendorId", schema: "payments", table: "Payments", column: "VendorId");
            migrationBuilder.CreateIndex(name: "IX_Payments_ApartmentId", schema: "payments", table: "Payments", column: "ApartmentId");
            migrationBuilder.CreateIndex(name: "IX_Payments_GarageId", schema: "payments", table: "Payments", column: "GarageId");

            migrationBuilder.CreateIndex(name: "IX_Charges_OwnerId", schema: "payments", table: "Charges", column: "OwnerId");
            migrationBuilder.CreateIndex(name: "IX_Charges_VendorId", schema: "payments", table: "Charges", column: "VendorId");
            migrationBuilder.CreateIndex(name: "IX_Charges_ApartmentId", schema: "payments", table: "Charges", column: "ApartmentId");
            migrationBuilder.CreateIndex(name: "IX_Charges_GarageId", schema: "payments", table: "Charges", column: "GarageId");
            migrationBuilder.CreateIndex(
                name: "IX_Charges_OwnerId_IssuedOn", schema: "payments", table: "Charges", columns: new[] { "OwnerId", "IssuedOn" });
            migrationBuilder.CreateIndex(
                name: "IX_Charges_VendorId_IssuedOn", schema: "payments", table: "Charges", columns: new[] { "VendorId", "IssuedOn" });
            migrationBuilder.CreateIndex(
                name: "IX_Charges_OwnerId_ApartmentId_Period",
                schema: "payments",
                table: "Charges",
                columns: new[] { "OwnerId", "ApartmentId", "Period" },
                unique: true,
                filter: "\"ApartmentId\" IS NOT NULL AND \"DeletedAt\" IS NULL");
            migrationBuilder.CreateIndex(
                name: "IX_Charges_OwnerId_GarageId_Period",
                schema: "payments",
                table: "Charges",
                columns: new[] { "OwnerId", "GarageId", "Period" },
                unique: true,
                filter: "\"GarageId\" IS NOT NULL AND \"DeletedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Payments_OwnerId", schema: "payments", table: "Payments");
            migrationBuilder.DropIndex(name: "IX_Payments_VendorId", schema: "payments", table: "Payments");
            migrationBuilder.DropIndex(name: "IX_Payments_ApartmentId", schema: "payments", table: "Payments");
            migrationBuilder.DropIndex(name: "IX_Payments_GarageId", schema: "payments", table: "Payments");

            migrationBuilder.DropIndex(name: "IX_Charges_OwnerId", schema: "payments", table: "Charges");
            migrationBuilder.DropIndex(name: "IX_Charges_VendorId", schema: "payments", table: "Charges");
            migrationBuilder.DropIndex(name: "IX_Charges_ApartmentId", schema: "payments", table: "Charges");
            migrationBuilder.DropIndex(name: "IX_Charges_GarageId", schema: "payments", table: "Charges");
            migrationBuilder.DropIndex(name: "IX_Charges_OwnerId_IssuedOn", schema: "payments", table: "Charges");
            migrationBuilder.DropIndex(name: "IX_Charges_VendorId_IssuedOn", schema: "payments", table: "Charges");
            migrationBuilder.DropIndex(name: "IX_Charges_OwnerId_ApartmentId_Period", schema: "payments", table: "Charges");
            migrationBuilder.DropIndex(name: "IX_Charges_OwnerId_GarageId_Period", schema: "payments", table: "Charges");

            migrationBuilder.AddColumn<Guid>(
                name: "PartyId", schema: "payments", table: "Payments", type: "uuid", nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "PartyType", schema: "payments", table: "Payments", type: "character varying(50)", maxLength: 50, nullable: true);
            migrationBuilder.AddColumn<Guid>(
                name: "PropertyId", schema: "payments", table: "Payments", type: "uuid", nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "PropertyType", schema: "payments", table: "Payments", type: "character varying(50)", maxLength: 50, nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PartyId", schema: "payments", table: "Charges", type: "uuid", nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "PartyType", schema: "payments", table: "Charges", type: "character varying(50)", maxLength: 50, nullable: true);
            migrationBuilder.AddColumn<Guid>(
                name: "PropertyId", schema: "payments", table: "Charges", type: "uuid", nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "PropertyType", schema: "payments", table: "Charges", type: "character varying(50)", maxLength: 50, nullable: true);

            migrationBuilder.Sql(
                "UPDATE payments.\"Payments\" SET \"PartyType\" = 'Owner', \"PartyId\" = \"OwnerId\" WHERE \"OwnerId\" IS NOT NULL;");
            migrationBuilder.Sql(
                "UPDATE payments.\"Payments\" SET \"PartyType\" = 'Vendor', \"PartyId\" = \"VendorId\" WHERE \"VendorId\" IS NOT NULL;");
            migrationBuilder.Sql(
                "UPDATE payments.\"Payments\" SET \"PropertyType\" = 'Apartment', \"PropertyId\" = \"ApartmentId\" WHERE \"ApartmentId\" IS NOT NULL;");
            migrationBuilder.Sql(
                "UPDATE payments.\"Payments\" SET \"PropertyType\" = 'Garage', \"PropertyId\" = \"GarageId\" WHERE \"GarageId\" IS NOT NULL;");

            migrationBuilder.Sql(
                "UPDATE payments.\"Charges\" SET \"PartyType\" = 'Owner', \"PartyId\" = \"OwnerId\" WHERE \"OwnerId\" IS NOT NULL;");
            migrationBuilder.Sql(
                "UPDATE payments.\"Charges\" SET \"PartyType\" = 'Vendor', \"PartyId\" = \"VendorId\" WHERE \"VendorId\" IS NOT NULL;");
            migrationBuilder.Sql(
                "UPDATE payments.\"Charges\" SET \"PropertyType\" = 'Apartment', \"PropertyId\" = \"ApartmentId\" WHERE \"ApartmentId\" IS NOT NULL;");
            migrationBuilder.Sql(
                "UPDATE payments.\"Charges\" SET \"PropertyType\" = 'Garage', \"PropertyId\" = \"GarageId\" WHERE \"GarageId\" IS NOT NULL;");

            migrationBuilder.AlterColumn<Guid>(
                name: "PartyId", schema: "payments", table: "Payments", type: "uuid", nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
            migrationBuilder.AlterColumn<string>(
                name: "PartyType", schema: "payments", table: "Payments", type: "character varying(50)", maxLength: 50, nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "PartyId", schema: "payments", table: "Charges", type: "uuid", nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
            migrationBuilder.AlterColumn<string>(
                name: "PartyType", schema: "payments", table: "Charges", type: "character varying(50)", maxLength: 50, nullable: false,
                defaultValue: "");

            migrationBuilder.DropColumn(name: "OwnerId", schema: "payments", table: "Payments");
            migrationBuilder.DropColumn(name: "VendorId", schema: "payments", table: "Payments");
            migrationBuilder.DropColumn(name: "ApartmentId", schema: "payments", table: "Payments");
            migrationBuilder.DropColumn(name: "GarageId", schema: "payments", table: "Payments");

            migrationBuilder.DropColumn(name: "OwnerId", schema: "payments", table: "Charges");
            migrationBuilder.DropColumn(name: "VendorId", schema: "payments", table: "Charges");
            migrationBuilder.DropColumn(name: "ApartmentId", schema: "payments", table: "Charges");
            migrationBuilder.DropColumn(name: "GarageId", schema: "payments", table: "Charges");

            migrationBuilder.CreateIndex(name: "IX_Payments_PartyId", schema: "payments", table: "Payments", column: "PartyId");
            migrationBuilder.CreateIndex(name: "IX_Payments_PartyType", schema: "payments", table: "Payments", column: "PartyType");
            migrationBuilder.CreateIndex(name: "IX_Payments_PropertyId", schema: "payments", table: "Payments", column: "PropertyId");

            migrationBuilder.CreateIndex(name: "IX_Charges_PartyId", schema: "payments", table: "Charges", column: "PartyId");
            migrationBuilder.CreateIndex(name: "IX_Charges_PartyType", schema: "payments", table: "Charges", column: "PartyType");
            migrationBuilder.CreateIndex(name: "IX_Charges_PropertyId", schema: "payments", table: "Charges", column: "PropertyId");
            migrationBuilder.CreateIndex(
                name: "IX_Charges_PartyType_PartyId_IssuedOn", schema: "payments", table: "Charges", columns: new[] { "PartyType", "PartyId", "IssuedOn" });
            migrationBuilder.CreateIndex(
                name: "IX_Charges_PartyId_PropertyId_Period",
                schema: "payments",
                table: "Charges",
                columns: new[] { "PartyId", "PropertyId", "Period" },
                unique: true,
                filter: "\"PartyType\" = 'Owner' AND \"DeletedAt\" IS NULL");
        }
    }
}
