using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Warehouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseReceiptReferenceUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_warehouse_transactions_ReferenceType_ReferenceId_Nomenclatu~",
                schema: "warehouse",
                table: "warehouse_transactions",
                columns: new[] { "ReferenceType", "ReferenceId", "NomenclatureId" },
                unique: true,
                filter: "\"ReferenceType\" IS NOT NULL AND \"ReferenceId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_warehouse_transactions_ReferenceType_ReferenceId_Nomenclatu~",
                schema: "warehouse",
                table: "warehouse_transactions");
        }
    }
}
