using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChargeUniqueIndexesIgnoreCancelled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Charges_ContractServiceId_Period",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropIndex(
                name: "IX_Charges_OwnerId_ApartmentId_Period",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropIndex(
                name: "IX_Charges_OwnerId_GarageId_Period",
                schema: "payments",
                table: "Charges");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_ContractServiceId_Period",
                schema: "payments",
                table: "Charges",
                columns: new[] { "ContractServiceId", "Period" },
                unique: true,
                filter: "\"ContractServiceId\" IS NOT NULL AND \"DeletedAt\" IS NULL AND \"Status\" <> 'Cancelled'");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_OwnerId_ApartmentId_Period",
                schema: "payments",
                table: "Charges",
                columns: new[] { "OwnerId", "ApartmentId", "Period" },
                unique: true,
                filter: "\"ApartmentId\" IS NOT NULL AND \"DeletedAt\" IS NULL AND \"Status\" <> 'Cancelled'");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_OwnerId_GarageId_Period",
                schema: "payments",
                table: "Charges",
                columns: new[] { "OwnerId", "GarageId", "Period" },
                unique: true,
                filter: "\"GarageId\" IS NOT NULL AND \"DeletedAt\" IS NULL AND \"Status\" <> 'Cancelled'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Charges_ContractServiceId_Period",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropIndex(
                name: "IX_Charges_OwnerId_ApartmentId_Period",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropIndex(
                name: "IX_Charges_OwnerId_GarageId_Period",
                schema: "payments",
                table: "Charges");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_ContractServiceId_Period",
                schema: "payments",
                table: "Charges",
                columns: new[] { "ContractServiceId", "Period" },
                unique: true,
                filter: "\"ContractServiceId\" IS NOT NULL AND \"DeletedAt\" IS NULL");

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
    }
}
