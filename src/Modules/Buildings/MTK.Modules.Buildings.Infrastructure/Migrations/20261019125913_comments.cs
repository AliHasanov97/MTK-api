using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Buildings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class comments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "appeals",
                schema: "buildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    SubmitterId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedToId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    GarageId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ClosedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appeals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "appeal_comments",
                schema: "buildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppealId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appeal_comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_appeal_comments_appeals_AppealId",
                        column: x => x.AppealId,
                        principalSchema: "buildings",
                        principalTable: "appeals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appeal_comments_AppealId",
                schema: "buildings",
                table: "appeal_comments",
                column: "AppealId");

            migrationBuilder.CreateIndex(
                name: "IX_appeal_comments_CreatedAt",
                schema: "buildings",
                table: "appeal_comments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_appeal_comments_UserId",
                schema: "buildings",
                table: "appeal_comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_appeals_AssignedToId",
                schema: "buildings",
                table: "appeals",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_appeals_Category",
                schema: "buildings",
                table: "appeals",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_appeals_CreatedAt",
                schema: "buildings",
                table: "appeals",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_appeals_Status",
                schema: "buildings",
                table: "appeals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_appeals_SubmitterId",
                schema: "buildings",
                table: "appeals",
                column: "SubmitterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appeal_comments",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "appeals",
                schema: "buildings");
        }
    }
}
