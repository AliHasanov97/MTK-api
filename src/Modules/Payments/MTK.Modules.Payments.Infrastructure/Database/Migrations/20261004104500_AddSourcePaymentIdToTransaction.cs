using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSourcePaymentIdToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SourcePaymentId",
                schema: "payments",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SourcePaymentId",
                schema: "payments",
                table: "Transactions",
                column: "SourcePaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Payments_SourcePaymentId",
                schema: "payments",
                table: "Transactions",
                column: "SourcePaymentId",
                principalSchema: "payments",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Payments_SourcePaymentId",
                schema: "payments",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SourcePaymentId",
                schema: "payments",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SourcePaymentId",
                schema: "payments",
                table: "Transactions");
        }
    }
}
