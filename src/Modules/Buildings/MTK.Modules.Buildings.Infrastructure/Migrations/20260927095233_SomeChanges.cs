using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace MTK.Modules.Buildings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SomeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "buildings",
                table: "OwnershipHistories",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "PreviousOwnerName", "NewOwnerName", "Notes" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "buildings",
                table: "Owners",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "FirstName", "LastName", "Email", "PhoneNumber" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "buildings",
                table: "Garages",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "GarageNumber", "Type", "Description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "buildings",
                table: "Buildings",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Name", "Description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "buildings",
                table: "AuditLogs",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "EntityType", "Action" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "buildings",
                table: "Apartments",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "ApartmentNumber", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_OwnershipHistories_SearchVector",
                schema: "buildings",
                table: "OwnershipHistories",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_Owners_SearchVector",
                schema: "buildings",
                table: "Owners",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_Garages_SearchVector",
                schema: "buildings",
                table: "Garages",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_SearchVector",
                schema: "buildings",
                table: "Buildings",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_SearchVector",
                schema: "buildings",
                table: "AuditLogs",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_SearchVector",
                schema: "buildings",
                table: "Apartments",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OwnershipHistories_SearchVector",
                schema: "buildings",
                table: "OwnershipHistories");

            migrationBuilder.DropIndex(
                name: "IX_Owners_SearchVector",
                schema: "buildings",
                table: "Owners");

            migrationBuilder.DropIndex(
                name: "IX_Garages_SearchVector",
                schema: "buildings",
                table: "Garages");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_SearchVector",
                schema: "buildings",
                table: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_SearchVector",
                schema: "buildings",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_SearchVector",
                schema: "buildings",
                table: "Apartments");

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "buildings",
                table: "OwnershipHistories",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "PreviousOwnerName", "NewOwnerName", "Notes" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "buildings",
                table: "Owners",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "FirstName", "LastName", "Email", "PhoneNumber" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "buildings",
                table: "Garages",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "GarageNumber", "Type", "Description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "buildings",
                table: "Buildings",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "Name", "Description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "buildings",
                table: "AuditLogs",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "EntityType", "Action" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "buildings",
                table: "Apartments",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "ApartmentNumber", "Status" });
        }
    }
}
