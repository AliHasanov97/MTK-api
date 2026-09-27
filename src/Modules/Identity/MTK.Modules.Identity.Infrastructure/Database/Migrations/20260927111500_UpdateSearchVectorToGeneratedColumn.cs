using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Identity.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSearchVectorToGeneratedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var tables = new[] { "Users", "AuditLogs" };

            foreach (var table in tables)
            {
                // Drop the existing SearchVector column
                migrationBuilder.Sql($@"
                    ALTER TABLE identity.""{table}""
                    DROP COLUMN ""SearchVector"";
                ");
            }

            // Add SearchVector as GENERATED column for Users
            migrationBuilder.Sql(@"
                ALTER TABLE identity.""Users""
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

            // Create GIN index for Users.SearchVector
            migrationBuilder.Sql(@"
                CREATE INDEX ""IX_Users_SearchVector""
                ON identity.""Users""
                USING GIN (""SearchVector"");
            ");

            // Add SearchVector as GENERATED column for AuditLogs
            migrationBuilder.Sql(@"
                ALTER TABLE identity.""AuditLogs""
                ADD COLUMN ""SearchVector"" tsvector
                GENERATED ALWAYS AS (
                    to_tsvector('english',
                        coalesce(""EntityType"", '') || ' ' ||
                        coalesce(""Action"", '')
                    )
                ) STORED;
            ");

            // Create GIN index for AuditLogs.SearchVector
            migrationBuilder.Sql(@"
                CREATE INDEX ""IX_AuditLogs_SearchVector""
                ON identity.""AuditLogs""
                USING GIN (""SearchVector"");
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS identity.""IX_Users_SearchVector"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS identity.""IX_AuditLogs_SearchVector"";");

            var tables = new[] { "Users", "AuditLogs" };

            foreach (var table in tables)
            {
                // Drop generated SearchVector column
                migrationBuilder.Sql($@"
                    ALTER TABLE identity.""{table}""
                    DROP COLUMN ""SearchVector"";
                ");

                // Add back as regular NOT NULL column
                migrationBuilder.Sql($@"
                    ALTER TABLE identity.""{table}""
                    ADD COLUMN ""SearchVector"" tsvector NULL;
                ");

                migrationBuilder.Sql($@"
                    UPDATE identity.""{table}""
                    SET ""SearchVector"" = to_tsvector('english', '');
                ");

                migrationBuilder.Sql($@"
                    ALTER TABLE identity.""{table}""
                    ALTER COLUMN ""SearchVector"" SET NOT NULL;
                ");
            }
        }
    }
}
