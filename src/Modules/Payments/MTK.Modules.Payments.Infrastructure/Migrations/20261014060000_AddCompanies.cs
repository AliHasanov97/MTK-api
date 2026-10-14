using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "companies",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Director = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.Id);
                });

            // PaymentReceiptExportService/ExportPayrollRunQueryHandler/ExportPayslipQueryHandler-da
            // əvvəllər hardcoded olan dəyər ilə seed edilir ki, deploy-dan sonra PDF-lərdə görünən ad
            // dəyişməsin. Admin panel-dən (PUT /api/payments/companies) sonra istənilən vaxt yenilənə bilər.
            migrationBuilder.InsertData(
                schema: "payments",
                table: "companies",
                columns: new[] { "Id", "Name", "Director", "Address", "CreatedAt" },
                values: new object[]
                {
                    new Guid("3f6f9b9a-2a9c-4b7a-8b9b-1f9a3e9c7a21"),
                    "Yusifoğlu İnşaat",
                    null,
                    "Bakı şəhəri",
                    new DateTimeOffset(2026, 10, 14, 0, 0, 0, TimeSpan.Zero)
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "companies",
                schema: "payments");
        }
    }
}
