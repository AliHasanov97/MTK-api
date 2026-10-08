using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MTK.Modules.Payments.Infrastructure.Database;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations;

[DbContext(typeof(PaymentsDbContext))]
[Migration("20261008100000_AddPurchaseExpenseTransaction")]
public partial class AddPurchaseExpenseTransaction : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "SourcePurchaseId",
            schema: "payments",
            table: "Transactions",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Transactions_SourcePurchaseId",
            schema: "payments",
            table: "Transactions",
            column: "SourcePurchaseId",
            unique: true,
            filter: "\"SourcePurchaseId\" IS NOT NULL");

        migrationBuilder.AddForeignKey(
            name: "FK_Transactions_Purchases_SourcePurchaseId",
            schema: "payments",
            table: "Transactions",
            column: "SourcePurchaseId",
            principalSchema: "payments",
            principalTable: "Purchases",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Transactions_Purchases_SourcePurchaseId",
            schema: "payments",
            table: "Transactions");

        migrationBuilder.DropIndex(
            name: "IX_Transactions_SourcePurchaseId",
            schema: "payments",
            table: "Transactions");

        migrationBuilder.DropColumn(
            name: "SourcePurchaseId",
            schema: "payments",
            table: "Transactions");
    }
}
