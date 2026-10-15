using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayslipPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PaidAt",
                schema: "payments",
                table: "payslips",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                schema: "payments",
                table: "payslips",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourcePayslipId",
                schema: "payments",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SourcePayslipId",
                schema: "payments",
                table: "Transactions",
                column: "SourcePayslipId",
                unique: true,
                filter: "\"SourcePayslipId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_payslips_SourcePayslipId",
                schema: "payments",
                table: "Transactions",
                column: "SourcePayslipId",
                principalSchema: "payments",
                principalTable: "payslips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_payslips_SourcePayslipId",
                schema: "payments",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SourcePayslipId",
                schema: "payments",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SourcePayslipId",
                schema: "payments",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                schema: "payments",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                schema: "payments",
                table: "payslips");
        }
    }
}
