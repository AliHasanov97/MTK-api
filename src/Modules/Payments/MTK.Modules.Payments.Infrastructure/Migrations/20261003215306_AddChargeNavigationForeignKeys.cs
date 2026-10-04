using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChargeNavigationForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Charges_Vendors_VendorId",
                schema: "payments",
                table: "Charges",
                column: "VendorId",
                principalSchema: "payments",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Charges_apartments_ApartmentId",
                schema: "payments",
                table: "Charges",
                column: "ApartmentId",
                principalSchema: "payments",
                principalTable: "apartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Charges_garages_GarageId",
                schema: "payments",
                table: "Charges",
                column: "GarageId",
                principalSchema: "payments",
                principalTable: "garages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Charges_owners_OwnerId",
                schema: "payments",
                table: "Charges",
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
                name: "FK_Charges_Vendors_VendorId",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropForeignKey(
                name: "FK_Charges_apartments_ApartmentId",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropForeignKey(
                name: "FK_Charges_garages_GarageId",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropForeignKey(
                name: "FK_Charges_owners_OwnerId",
                schema: "payments",
                table: "Charges");
        }
    }
}
