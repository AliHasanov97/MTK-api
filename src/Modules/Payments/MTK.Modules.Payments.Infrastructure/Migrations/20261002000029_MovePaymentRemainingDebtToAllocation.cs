using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MovePaymentRemainingDebtToAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingDebtAfterPayment",
                schema: "payments",
                table: "Payments");

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingDebtAfterPayment",
                schema: "payments",
                table: "PaymentAllocations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingDebtAfterPayment",
                schema: "payments",
                table: "PaymentAllocations");

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingDebtAfterPayment",
                schema: "payments",
                table: "Payments",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }
    }
}
