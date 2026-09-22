using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MTK.Modules.Identity.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxInboxToIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "identity",
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Description", "IsActive", "KeycloakSyncStatus", "LastSyncError", "Name", "RoleType", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sistem administratoru - tam giriş hüququ", true, "PendingSync", null, "SystemAdmin", 1, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "Bina idarəçisi (Komandant) - bina əməliyyatları və maliyyəsini idarə edir", true, "PendingSync", null, "BuildingManager", 1, null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mühasib - maliyyə əməliyyatları və hesabatları idarə edir", true, "PendingSync", null, "Accountant", 1, null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mənzil sahibi - bir və ya bir neçə mənzilin sahibi olan sakin", true, "PendingSync", null, "ApartmentOwner", 1, null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "İşçi - təmizlikçi, mühafizəçi, texniki işçilər", true, "PendingSync", null, "Employee", 1, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));
        }
    }
}
