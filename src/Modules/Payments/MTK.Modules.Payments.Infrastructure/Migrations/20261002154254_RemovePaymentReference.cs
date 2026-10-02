using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePaymentReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // EF's own AlterColumn<NpgsqlTsVector> generator throws
            // ("refers to unknown column in tsvector definition") when shrinking a
            // GENERATED ALWAYS tsvector column's property list via migrations — a
            // provider-level limitation, not something expressible as a plain
            // builder call. Raw SQL instead: drop the generated column (also drops
            // its GIN index), drop Reference, then recreate SearchVector without it.
            migrationBuilder.Sql(
                """
                ALTER TABLE payments."Payments" DROP COLUMN "SearchVector";
                ALTER TABLE payments."Payments" DROP COLUMN "Reference";
                ALTER TABLE payments."Payments"
                    ADD COLUMN "SearchVector" tsvector
                    GENERATED ALWAYS AS (to_tsvector('english'::regconfig, COALESCE("Notes", ''::character varying)::text)) STORED NOT NULL;
                CREATE INDEX "IX_Payments_SearchVector" ON payments."Payments" USING GIN ("SearchVector");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE payments."Payments" DROP COLUMN "SearchVector";
                ALTER TABLE payments."Payments" ADD COLUMN "Reference" character varying(200);
                ALTER TABLE payments."Payments"
                    ADD COLUMN "SearchVector" tsvector
                    GENERATED ALWAYS AS (to_tsvector('english'::regconfig, (COALESCE("Reference", ''::character varying)::text || ' '::text) || COALESCE("Notes", ''::character varying)::text)) STORED NOT NULL;
                CREATE INDEX "IX_Payments_SearchVector" ON payments."Payments" USING GIN ("SearchVector");
                CREATE INDEX "IX_Payments_Reference" ON payments."Payments" ("Reference");
                """);
        }
    }
}
