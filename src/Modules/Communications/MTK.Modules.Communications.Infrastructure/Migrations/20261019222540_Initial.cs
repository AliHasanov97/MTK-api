using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace MTK.Modules.Communications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "communications");

            migrationBuilder.CreateTable(
                name: "inbox_message_consumers",
                schema: "communications",
                columns: table => new
                {
                    InboxMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inbox_message_consumers", x => new { x.InboxMessageId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "inbox_messages",
                schema: "communications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "jsonb", maxLength: 2000, nullable: false),
                    OccurredOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProcessedOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inbox_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_message_consumers",
                schema: "communications",
                columns: table => new
                {
                    OutboxMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_message_consumers", x => new { x.OutboxMessageId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "communications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "jsonb", maxLength: 2000, nullable: false),
                    OccurredOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProcessedOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true),
                    Attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NextAttemptOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TriggeredByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "communications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IdentityId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "FirstName", "LastName", "Email" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "articles",
                schema: "communications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Summary = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Draft"),
                    PublishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('simple', coalesce(\"Title\", '') || ' ' || coalesce(\"Summary\", '') || ' ' || coalesce(\"Content\", ''))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_articles_users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "communications",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_articles_CreatedAt",
                schema: "communications",
                table: "articles",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_articles_CreatedById",
                schema: "communications",
                table: "articles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_articles_PublishedAt",
                schema: "communications",
                table: "articles",
                column: "PublishedAt");

            migrationBuilder.CreateIndex(
                name: "IX_articles_SearchVector",
                schema: "communications",
                table: "articles",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_articles_Status",
                schema: "communications",
                table: "articles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                schema: "communications",
                table: "users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_users_IdentityId",
                schema: "communications",
                table: "users",
                column: "IdentityId",
                filter: "\"IdentityId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_SearchVector",
                schema: "communications",
                table: "users",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "articles",
                schema: "communications");

            migrationBuilder.DropTable(
                name: "inbox_message_consumers",
                schema: "communications");

            migrationBuilder.DropTable(
                name: "inbox_messages",
                schema: "communications");

            migrationBuilder.DropTable(
                name: "outbox_message_consumers",
                schema: "communications");

            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "communications");

            migrationBuilder.DropTable(
                name: "users",
                schema: "communications");
        }
    }
}
