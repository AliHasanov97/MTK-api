using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerContractsAndCharges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                schema: "payments",
                table: "Payments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "VendorId",
                schema: "payments",
                table: "Contracts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                schema: "payments",
                table: "Contracts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                schema: "payments",
                table: "Charges",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_CustomerId",
                schema: "payments",
                table: "Contracts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_CustomerId_IssuedOn",
                schema: "payments",
                table: "Charges",
                columns: new[] { "CustomerId", "IssuedOn" });

            migrationBuilder.AddForeignKey(
                name: "FK_Charges_Customers_CustomerId",
                schema: "payments",
                table: "Charges",
                column: "CustomerId",
                principalSchema: "payments",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Charges_Customers_CustomerId",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_CustomerId",
                schema: "payments",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Charges_CustomerId_IssuedOn",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "payments",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "payments",
                table: "Charges");

            migrationBuilder.AlterColumn<Guid>(
                name: "VendorId",
                schema: "payments",
                table: "Contracts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
