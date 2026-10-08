using System;
using MTK.Modules.Payments.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations;

[DbContext(typeof(PaymentsDbContext))]
[Migration("20261008130000_AddPurchaseIdToFileAttachment")]
public partial class AddPurchaseIdToFileAttachment : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "PurchaseId",
            schema: "payments",
            table: "FileAttachments",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_FileAttachments_PurchaseId",
            schema: "payments",
            table: "FileAttachments",
            column: "PurchaseId");

        migrationBuilder.AddForeignKey(
            name: "FK_FileAttachments_Purchases_PurchaseId",
            schema: "payments",
            table: "FileAttachments",
            column: "PurchaseId",
            principalSchema: "payments",
            principalTable: "Purchases",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_FileAttachments_Purchases_PurchaseId",
            schema: "payments",
            table: "FileAttachments");

        migrationBuilder.DropIndex(
            name: "IX_FileAttachments_PurchaseId",
            schema: "payments",
            table: "FileAttachments");

        migrationBuilder.DropColumn(
            name: "PurchaseId",
            schema: "payments",
            table: "FileAttachments");
    }
}
