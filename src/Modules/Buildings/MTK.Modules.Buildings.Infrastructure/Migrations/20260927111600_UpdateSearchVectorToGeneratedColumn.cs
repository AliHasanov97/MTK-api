using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Buildings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSearchVectorToGeneratedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var tables = new[] { "Apartments", "Buildings", "Garages", "Owners", "OwnershipHistories", "AuditLogs" };

            foreach (var table in tables)
            {
                // Drop the existing SearchVector column
                migrationBuilder.Sql($@"
                    ALTER TABLE buildings.""{table}""
                    DROP COLUMN ""SearchVector"";
                ");
            }

            // Add SearchVector as GENERATED column for Apartments
            migrationBuilder.Sql(@"
                ALTER TABLE buildings.""Apartments""
                ADD COLUMN ""SearchVector"" tsvector
                GENERATED ALWAYS AS (
                    to_tsvector('english',
                        coalesce(""ApartmentNumber"", '') || ' ' ||
                        coalesce(""Status"", '')
                    )
                ) STORED;
            ");

            // Create GIN index for Apartments.SearchVector
            migrationBuilder.Sql(@"
                CREATE INDEX ""IX_Apartments_SearchVector""
                ON buildings.""Apartments""
                USING GIN (""SearchVector"");
            ");

            // Add SearchVector as GENERATED column for Buildings
            migrationBuilder.Sql(@"
                ALTER TABLE buildings.""Buildings""
                ADD COLUMN ""SearchVector"" tsvector
                GENERATED ALWAYS AS (
                    to_tsvector('english',
                        coalesce(""Name"", '') || ' ' ||
                        coalesce(""Description"", '')
                    )
                ) STORED;
            ");

            // Create GIN index for Buildings.SearchVector
            migrationBuilder.Sql(@"
                CREATE INDEX ""IX_Buildings_SearchVector""
                ON buildings.""Buildings""
                USING GIN (""SearchVector"");
            ");

            // Add SearchVector as GENERATED column for Garages
            migrationBuilder.Sql(@"
                ALTER TABLE buildings.""Garages""
                ADD COLUMN ""SearchVector"" tsvector
                GENERATED ALWAYS AS (
                    to_tsvector('english',
                        coalesce(""GarageNumber"", '') || ' ' ||
                        coalesce(""Type"", '') || ' ' ||
                        coalesce(""Description"", '')
                    )
                ) STORED;
            ");

            // Create GIN index for Garages.SearchVector
            migrationBuilder.Sql(@"
                CREATE INDEX ""IX_Garages_SearchVector""
                ON buildings.""Garages""
                USING GIN (""SearchVector"");
            ");

            // Add SearchVector as GENERATED column for Owners
            migrationBuilder.Sql(@"
                ALTER TABLE buildings.""Owners""
                ADD COLUMN ""SearchVector"" tsvector
                GENERATED ALWAYS AS (
                    to_tsvector('english',
                        coalesce(""FirstName"", '') || ' ' ||
                        coalesce(""LastName"", '') || ' ' ||
                        coalesce(""Email"", '') || ' ' ||
                        coalesce(""PhoneNumber"", '')
                    )
                ) STORED;
            ");

            // Create GIN index for Owners.SearchVector
            migrationBuilder.Sql(@"
                CREATE INDEX ""IX_Owners_SearchVector""
                ON buildings.""Owners""
                USING GIN (""SearchVector"");
            ");

            // Add SearchVector as GENERATED column for OwnershipHistories
            migrationBuilder.Sql(@"
                ALTER TABLE buildings.""OwnershipHistories""
                ADD COLUMN ""SearchVector"" tsvector
                GENERATED ALWAYS AS (
                    to_tsvector('english', '')
                ) STORED;
            ");

            // Create GIN index for OwnershipHistories.SearchVector
            migrationBuilder.Sql(@"
                CREATE INDEX ""IX_OwnershipHistories_SearchVector""
                ON buildings.""OwnershipHistories""
                USING GIN (""SearchVector"");
            ");

            // Add SearchVector as GENERATED column for AuditLogs
            migrationBuilder.Sql(@"
                ALTER TABLE buildings.""AuditLogs""
                ADD COLUMN ""SearchVector"" tsvector
                GENERATED ALWAYS AS (
                    to_tsvector('english', '')
                ) STORED;
            ");

            // Create GIN index for AuditLogs.SearchVector
            migrationBuilder.Sql(@"
                CREATE INDEX ""IX_AuditLogs_SearchVector""
                ON buildings.""AuditLogs""
                USING GIN (""SearchVector"");
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS buildings.""IX_Apartments_SearchVector"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS buildings.""IX_Buildings_SearchVector"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS buildings.""IX_Garages_SearchVector"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS buildings.""IX_Owners_SearchVector"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS buildings.""IX_OwnershipHistories_SearchVector"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS buildings.""IX_AuditLogs_SearchVector"";");

            var tables = new[] { "Apartments", "Buildings", "Garages", "Owners", "OwnershipHistories", "AuditLogs" };

            foreach (var table in tables)
            {
                // Drop generated SearchVector column
                migrationBuilder.Sql($@"
                    ALTER TABLE buildings.""{table}""
                    DROP COLUMN ""SearchVector"";
                ");

                // Add back as regular NOT NULL column
                migrationBuilder.Sql($@"
                    ALTER TABLE buildings.""{table}""
                    ADD COLUMN ""SearchVector"" tsvector NULL;
                ");

                migrationBuilder.Sql($@"
                    UPDATE buildings.""{table}""
                    SET ""SearchVector"" = to_tsvector('english', '');
                ");

                migrationBuilder.Sql($@"
                    ALTER TABLE buildings.""{table}""
                    ALTER COLUMN ""SearchVector"" SET NOT NULL;
                ");
            }
        }
    }
}
