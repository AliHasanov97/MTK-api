using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Buildings.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddFileAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileAttachments",
                schema: "buildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ObjectKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    GarageId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileAttachments_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalSchema: "buildings",
                        principalTable: "Apartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileAttachments_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalSchema: "buildings",
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileAttachments_Garages_GarageId",
                        column: x => x.GarageId,
                        principalSchema: "buildings",
                        principalTable: "Garages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileAttachments_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "buildings",
                        principalTable: "Owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_ApartmentId",
                schema: "buildings",
                table: "FileAttachments",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_BuildingId",
                schema: "buildings",
                table: "FileAttachments",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_GarageId",
                schema: "buildings",
                table: "FileAttachments",
                column: "GarageId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_OwnerId",
                schema: "buildings",
                table: "FileAttachments",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileAttachments",
                schema: "buildings");
        }
    }
}
