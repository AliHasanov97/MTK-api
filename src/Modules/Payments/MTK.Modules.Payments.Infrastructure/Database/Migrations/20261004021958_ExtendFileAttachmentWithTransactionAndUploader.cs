using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ExtendFileAttachmentWithTransactionAndUploader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                schema: "payments",
                table: "FileAttachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UploadedByUserId",
                schema: "payments",
                table: "FileAttachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_TransactionId",
                schema: "payments",
                table: "FileAttachments",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_UploadedByUserId",
                schema: "payments",
                table: "FileAttachments",
                column: "UploadedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileAttachments_Transactions_TransactionId",
                schema: "payments",
                table: "FileAttachments",
                column: "TransactionId",
                principalSchema: "payments",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FileAttachments_Users_UploadedByUserId",
                schema: "payments",
                table: "FileAttachments",
                column: "UploadedByUserId",
                principalSchema: "payments",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileAttachments_Transactions_TransactionId",
                schema: "payments",
                table: "FileAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_FileAttachments_Users_UploadedByUserId",
                schema: "payments",
                table: "FileAttachments");

            migrationBuilder.DropIndex(
                name: "IX_FileAttachments_TransactionId",
                schema: "payments",
                table: "FileAttachments");

            migrationBuilder.DropIndex(
                name: "IX_FileAttachments_UploadedByUserId",
                schema: "payments",
                table: "FileAttachments");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                schema: "payments",
                table: "FileAttachments");

            migrationBuilder.DropColumn(
                name: "UploadedByUserId",
                schema: "payments",
                table: "FileAttachments");
        }
    }
}
