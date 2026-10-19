using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace MTK.Modules.Communications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "communications",
                table: "articles");

            migrationBuilder.CreateTable(
                name: "file_attachments",
                schema: "communications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('simple', coalesce(\"FileName\", ''))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_file_attachments_articles_ArticleId",
                        column: x => x.ArticleId,
                        principalSchema: "communications",
                        principalTable: "articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_ArticleId",
                schema: "communications",
                table: "file_attachments",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_file_attachments_SearchVector",
                schema: "communications",
                table: "file_attachments",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "gin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "file_attachments",
                schema: "communications");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "communications",
                table: "articles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
