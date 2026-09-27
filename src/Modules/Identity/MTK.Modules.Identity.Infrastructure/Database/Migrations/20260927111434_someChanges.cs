using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace MTK.Modules.Identity.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class someChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IdentityId",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_KeycloakSyncStatus",
                schema: "identity",
                table: "Users");

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "identity",
                table: "Users",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "FirstName", "LastName", "Email", "PhoneNumber" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "identity",
                table: "AuditLogs",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "EntityType", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "identity",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdentityId",
                schema: "identity",
                table: "Users",
                column: "IdentityId",
                filter: "\"IdentityId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_KeycloakSyncStatus",
                schema: "identity",
                table: "Users",
                column: "KeycloakSyncStatus",
                filter: "\"KeycloakSyncStatus\" != 'Synced'");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SearchVector",
                schema: "identity",
                table: "Users",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_SearchVector",
                schema: "identity",
                table: "AuditLogs",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IdentityId",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_KeycloakSyncStatus",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SearchVector",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_SearchVector",
                schema: "identity",
                table: "AuditLogs");

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "identity",
                table: "Users",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "FirstName", "LastName", "Email", "PhoneNumber" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "identity",
                table: "AuditLogs",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "EntityType", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "identity",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[DeletedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdentityId",
                schema: "identity",
                table: "Users",
                column: "IdentityId",
                filter: "[IdentityId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_KeycloakSyncStatus",
                schema: "identity",
                table: "Users",
                column: "KeycloakSyncStatus",
                filter: "[KeycloakSyncStatus] != 'Synced'");
        }
    }
}
