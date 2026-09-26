using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace MTK.Modules.Buildings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchVectorToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add timestamp columns to OwnershipHistories
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                schema: "buildings",
                table: "OwnershipHistories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "buildings",
                table: "OwnershipHistories",
                type: "timestamp with time zone",
                nullable: true);

            // Add timestamp columns to AuditLogs
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "buildings",
                table: "AuditLogs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                schema: "buildings",
                table: "AuditLogs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "buildings",
                table: "AuditLogs",
                type: "timestamp with time zone",
                nullable: true);

            // Add SearchVector columns to all tables with proper handling for existing data
            var tables = new[] { "Apartments", "Buildings", "Garages", "Owners", "OwnershipHistories", "AuditLogs" };

            foreach (var table in tables)
            {
                // Step 1: Add column as nullable
                migrationBuilder.Sql($@"
                    ALTER TABLE buildings.""{table}""
                    ADD COLUMN ""SearchVector"" tsvector NULL;
                ");

                // Step 2: Set default value for existing rows
                migrationBuilder.Sql($@"
                    UPDATE buildings.""{table}""
                    SET ""SearchVector"" = to_tsvector('english', '');
                ");

                // Step 3: Make column NOT NULL
                migrationBuilder.Sql($@"
                    ALTER TABLE buildings.""{table}""
                    ALTER COLUMN ""SearchVector"" SET NOT NULL;
                ");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "buildings",
                table: "OwnershipHistories");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                schema: "buildings",
                table: "OwnershipHistories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "buildings",
                table: "OwnershipHistories");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                schema: "buildings",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                schema: "buildings",
                table: "Garages");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                schema: "buildings",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "buildings",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "buildings",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                schema: "buildings",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "buildings",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                schema: "buildings",
                table: "Apartments");
        }
    }
}
