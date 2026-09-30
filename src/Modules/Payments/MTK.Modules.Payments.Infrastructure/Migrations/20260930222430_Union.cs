using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Union : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_Charges_OwnerId_IssuedOn",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropIndex(
                name: "IX_Charges_OwnerId_PropertyId_Period",
                schema: "payments",
                table: "Charges");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "payments",
                table: "Payments",
                newName: "PartyId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_OwnerId",
                schema: "payments",
                table: "Payments",
                newName: "IX_Payments_PartyId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "payments",
                table: "Charges",
                newName: "PartyId");

            migrationBuilder.RenameIndex(
                name: "IX_Charges_OwnerId",
                schema: "payments",
                table: "Charges",
                newName: "IX_Charges_PartyId");

            migrationBuilder.AddColumn<string>(
                name: "PartyType",
                schema: "payments",
                table: "Payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "payments",
                table: "Charges",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Period", "Description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "Period" });

            migrationBuilder.AlterColumn<string>(
                name: "RateType",
                schema: "payments",
                table: "Charges",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "RateAmount",
                schema: "payments",
                table: "Charges",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "PropertyType",
                schema: "payments",
                table: "Charges",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "PropertyId",
                schema: "payments",
                table: "Charges",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Period",
                schema: "payments",
                table: "Charges",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "ContractId",
                schema: "payments",
                table: "Charges",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContractServiceId",
                schema: "payments",
                table: "Charges",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DueDate",
                schema: "payments",
                table: "Charges",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartyType",
                schema: "payments",
                table: "Charges",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PartyType",
                schema: "payments",
                table: "Payments",
                column: "PartyType");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_ContractId",
                schema: "payments",
                table: "Charges",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_ContractServiceId",
                schema: "payments",
                table: "Charges",
                column: "ContractServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_ContractServiceId_Period",
                schema: "payments",
                table: "Charges",
                columns: new[] { "ContractServiceId", "Period" },
                unique: true,
                filter: "\"ContractServiceId\" IS NOT NULL AND \"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_DueDate",
                schema: "payments",
                table: "Charges",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_PartyId_PropertyId_Period",
                schema: "payments",
                table: "Charges",
                columns: new[] { "PartyId", "PropertyId", "Period" },
                unique: true,
                filter: "\"PartyType\" = 'Owner' AND \"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_PartyType",
                schema: "payments",
                table: "Charges",
                column: "PartyType");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_PartyType_PartyId_IssuedOn",
                schema: "payments",
                table: "Charges",
                columns: new[] { "PartyType", "PartyId", "IssuedOn" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_PartyType",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Charges_ContractId",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropIndex(
                name: "IX_Charges_ContractServiceId",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropIndex(
                name: "IX_Charges_ContractServiceId_Period",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropIndex(
                name: "IX_Charges_DueDate",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropIndex(
                name: "IX_Charges_PartyId_PropertyId_Period",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropIndex(
                name: "IX_Charges_PartyType",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropIndex(
                name: "IX_Charges_PartyType_PartyId_IssuedOn",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropColumn(
                name: "PartyType",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ContractId",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropColumn(
                name: "ContractServiceId",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropColumn(
                name: "DueDate",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropColumn(
                name: "PartyType",
                schema: "payments",
                table: "Charges");

            migrationBuilder.RenameColumn(
                name: "PartyId",
                schema: "payments",
                table: "Payments",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PartyId",
                schema: "payments",
                table: "Payments",
                newName: "IX_Payments_OwnerId");

            migrationBuilder.RenameColumn(
                name: "PartyId",
                schema: "payments",
                table: "Charges",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Charges_PartyId",
                schema: "payments",
                table: "Charges",
                newName: "IX_Charges_OwnerId");

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "payments",
                table: "Charges",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Period" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "Period", "Description" });

            migrationBuilder.AlterColumn<string>(
                name: "RateType",
                schema: "payments",
                table: "Charges",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RateAmount",
                schema: "payments",
                table: "Charges",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PropertyType",
                schema: "payments",
                table: "Charges",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PropertyId",
                schema: "payments",
                table: "Charges",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Period",
                schema: "payments",
                table: "Charges",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ContractGoodsItems",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgreedQuantity = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ContractId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    PaymentTermDays = table.Column<int>(type: "integer", nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
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
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CancellationReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ChargeDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ContractGoodsItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    ContractId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContractServiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DueDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaidAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Period = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "Description", "Reference" }),
                    Source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PaymentDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "Reference", "Notes" }),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    VendorChargeId = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorPayments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Charges_OwnerId_IssuedOn",
                schema: "payments",
                table: "Charges",
                columns: new[] { "OwnerId", "IssuedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Charges_OwnerId_PropertyId_Period",
                schema: "payments",
                table: "Charges",
                columns: new[] { "OwnerId", "PropertyId", "Period" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

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
    }
}
