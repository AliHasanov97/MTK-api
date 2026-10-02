using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Buildings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOwnershipHistorySalePriceAndNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Raw SQL instead of AlterColumn<NpgsqlTsVector>: Npgsql's migration
            // generator validates the tsvector's OLD column list ("Notes")
            // against the CURRENT model, which no longer declares Notes at all
            // once its C# property is removed — it throws "SearchVector refers
            // to unknown column" regardless of operation order. Doing the
            // drop/recreate by hand sidesteps that validation entirely.
            migrationBuilder.Sql(
                """
                DROP INDEX buildings."IX_OwnershipHistories_SearchVector";
                ALTER TABLE buildings."OwnershipHistories" DROP COLUMN "SearchVector";
                ALTER TABLE buildings."OwnershipHistories" DROP COLUMN "Notes";
                ALTER TABLE buildings."OwnershipHistories" DROP COLUMN "SalePrice";
                ALTER TABLE buildings."OwnershipHistories" ADD COLUMN "SearchVector" tsvector NOT NULL
                    GENERATED ALWAYS AS (to_tsvector('english', (COALESCE("PreviousOwnerName", '')::text || ' '::text) || "NewOwnerName"::text)) STORED;
                CREATE INDEX "IX_OwnershipHistories_SearchVector" ON buildings."OwnershipHistories" USING GIN ("SearchVector");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP INDEX buildings."IX_OwnershipHistories_SearchVector";
                ALTER TABLE buildings."OwnershipHistories" DROP COLUMN "SearchVector";
                ALTER TABLE buildings."OwnershipHistories" ADD COLUMN "Notes" character varying(1000) NULL;
                ALTER TABLE buildings."OwnershipHistories" ADD COLUMN "SalePrice" numeric(18,2) NULL;
                ALTER TABLE buildings."OwnershipHistories" ADD COLUMN "SearchVector" tsvector NOT NULL
                    GENERATED ALWAYS AS (to_tsvector('english', ((COALESCE("PreviousOwnerName", '')::text || ' '::text) || "NewOwnerName"::text) || ' '::text || COALESCE("Notes", '')::text)) STORED;
                CREATE INDEX "IX_OwnershipHistories_SearchVector" ON buildings."OwnershipHistories" USING GIN ("SearchVector");
                """);
        }
    }
}
