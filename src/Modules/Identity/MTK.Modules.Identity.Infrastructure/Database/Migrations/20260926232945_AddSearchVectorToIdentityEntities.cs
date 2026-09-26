using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace MTK.Modules.Identity.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchVectorToIdentityEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add timestamp columns to AuditLogs
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "identity",
                table: "AuditLogs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                schema: "identity",
                table: "AuditLogs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "identity",
                table: "AuditLogs",
                type: "timestamp with time zone",
                nullable: true);

            // Add SearchVector columns to all tables with proper handling for existing data
            var tables = new[] { "Users", "AuditLogs" };

            foreach (var table in tables)
            {
                // Step 1: Add column as nullable
                migrationBuilder.Sql($@"
                    ALTER TABLE identity.""{table}""
                    ADD COLUMN ""SearchVector"" tsvector NULL;
                ");

                // Step 2: Set default value for existing rows
                migrationBuilder.Sql($@"
                    UPDATE identity.""{table}""
                    SET ""SearchVector"" = to_tsvector('english', '');
                ");

                // Step 3: Make column NOT NULL
                migrationBuilder.Sql($@"
                    ALTER TABLE identity.""{table}""
                    ALTER COLUMN ""SearchVector"" SET NOT NULL;
                ");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchVector",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "identity",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "identity",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                schema: "identity",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "identity",
                table: "AuditLogs");
        }
    }
}
