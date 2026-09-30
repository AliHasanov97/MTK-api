using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class somecjhnags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "payments",
                table: "Payments",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "IssuedOn",
                schema: "payments",
                table: "Charges",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "payments",
                table: "Charges",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "IX_Charges_OwnerId_IssuedOn",
                schema: "payments",
                table: "Charges",
                columns: new[] { "OwnerId", "IssuedOn" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Charges_PaidAmount_Range",
                schema: "payments",
                table: "Charges",
                sql: "\"PaidAmount\" >= 0 AND \"PaidAmount\" <= \"Amount\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Charges_OwnerId_IssuedOn",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Charges_PaidAmount_Range",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IssuedOn",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "payments",
                table: "Charges");
        }
    }
}
