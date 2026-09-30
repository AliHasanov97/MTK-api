using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropContractServiceShadowFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractServices_Contracts_ContractId1",
                schema: "payments",
                table: "ContractServices");

            migrationBuilder.DropIndex(
                name: "IX_ContractServices_ContractId1",
                schema: "payments",
                table: "ContractServices");

            migrationBuilder.DropColumn(
                name: "ContractId1",
                schema: "payments",
                table: "ContractServices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ContractId1",
                schema: "payments",
                table: "ContractServices",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractServices_ContractId1",
                schema: "payments",
                table: "ContractServices",
                column: "ContractId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractServices_Contracts_ContractId1",
                schema: "payments",
                table: "ContractServices",
                column: "ContractId1",
                principalSchema: "payments",
                principalTable: "Contracts",
                principalColumn: "Id");
        }
    }
}
