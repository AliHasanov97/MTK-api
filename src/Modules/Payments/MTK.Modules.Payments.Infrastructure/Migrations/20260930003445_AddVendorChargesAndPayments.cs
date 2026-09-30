using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorChargesAndPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentTermDays",
                schema: "payments",
                table: "ContractServices",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                schema: "payments",
                table: "ContractServices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ContractGoodsItems",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContractId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    AgreedQuantity = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    PaymentTermDays = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractGoodsItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractGoodsItems_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "payments",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorCharges",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContractId = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContractServiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    ContractGoodsItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    Period = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ChargeDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CancellationReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "Description", "Reference" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorCharges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendorPayments",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorChargeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "Reference", "Notes" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorPayments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractGoodsItems_ContractId",
                schema: "payments",
                table: "ContractGoodsItems",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractGoodsItems_IsActive",
                schema: "payments",
                table: "ContractGoodsItems",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCharges_ChargeDate",
                schema: "payments",
                table: "VendorCharges",
                column: "ChargeDate");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCharges_ContractGoodsItemId",
                schema: "payments",
                table: "VendorCharges",
                column: "ContractGoodsItemId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCharges_ContractGoodsItemId_Reference",
                schema: "payments",
                table: "VendorCharges",
                columns: new[] { "ContractGoodsItemId", "Reference" },
                unique: true,
                filter: "\"ContractGoodsItemId\" IS NOT NULL AND \"Reference\" IS NOT NULL AND \"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCharges_ContractId",
                schema: "payments",
                table: "VendorCharges",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCharges_ContractServiceId",
                schema: "payments",
                table: "VendorCharges",
                column: "ContractServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCharges_ContractServiceId_Period",
                schema: "payments",
                table: "VendorCharges",
                columns: new[] { "ContractServiceId", "Period" },
                unique: true,
                filter: "\"ContractServiceId\" IS NOT NULL AND \"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCharges_DueDate",
                schema: "payments",
                table: "VendorCharges",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCharges_SearchVector",
                schema: "payments",
                table: "VendorCharges",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCharges_Source",
                schema: "payments",
                table: "VendorCharges",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCharges_Status",
                schema: "payments",
                table: "VendorCharges",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCharges_VendorId",
                schema: "payments",
                table: "VendorCharges",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayments_PaymentDate",
                schema: "payments",
                table: "VendorPayments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayments_SearchVector",
                schema: "payments",
                table: "VendorPayments",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayments_Status",
                schema: "payments",
                table: "VendorPayments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayments_VendorChargeId",
                schema: "payments",
                table: "VendorPayments",
                column: "VendorChargeId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayments_VendorId",
                schema: "payments",
                table: "VendorPayments",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractGoodsItems",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "VendorCharges",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "VendorPayments",
                schema: "payments");

            migrationBuilder.DropColumn(
                name: "PaymentTermDays",
                schema: "payments",
                table: "ContractServices");

            migrationBuilder.DropColumn(
                name: "Unit",
                schema: "payments",
                table: "ContractServices");
        }
    }
}
