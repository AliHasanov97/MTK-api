using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTargetingAndGarageRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GarageType",
                schema: "payments",
                table: "Rates",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GarageType",
                schema: "payments",
                table: "property_ownerships",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PropertyId",
                schema: "payments",
                table: "Payments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyType",
                schema: "payments",
                table: "Payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            // Period is widened, but Postgres refuses to ALTER a column that a generated
            // (STORED) column depends on — SearchVector is generated from Period. Drop it
            // and its GIN index, widen the column, then recreate SearchVector identically.
            migrationBuilder.DropIndex(
                name: "IX_Charges_SearchVector",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                schema: "payments",
                table: "Charges");

            migrationBuilder.AlterColumn<string>(
                name: "Period",
                schema: "payments",
                table: "Charges",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "payments",
                table: "Charges",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "payments",
                table: "Charges",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Period" });

            migrationBuilder.CreateIndex(
                name: "IX_Charges_SearchVector",
                schema: "payments",
                table: "Charges",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_RateType_GarageType_EffectiveFrom",
                schema: "payments",
                table: "Rates",
                columns: new[] { "RateType", "GarageType", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PropertyId",
                schema: "payments",
                table: "Payments",
                column: "PropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rates_RateType_GarageType_EffectiveFrom",
                schema: "payments",
                table: "Rates");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PropertyId",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Charges_SearchVector",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                schema: "payments",
                table: "Charges");

            migrationBuilder.DropColumn(
                name: "GarageType",
                schema: "payments",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "GarageType",
                schema: "payments",
                table: "property_ownerships");

            migrationBuilder.DropColumn(
                name: "PropertyId",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PropertyType",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "payments",
                table: "Charges");

            migrationBuilder.AlterColumn<string>(
                name: "Period",
                schema: "payments",
                table: "Charges",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "payments",
                table: "Charges",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Period" });

            migrationBuilder.CreateIndex(
                name: "IX_Charges_SearchVector",
                schema: "payments",
                table: "Charges",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }
    }
}
