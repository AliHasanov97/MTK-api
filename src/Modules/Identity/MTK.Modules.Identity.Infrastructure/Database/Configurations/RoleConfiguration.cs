using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Identity.Domain.Keycloak;
using MTK.Modules.Identity.Domain.Roles;

namespace MTK.Modules.Identity.Infrastructure.Database.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", "identity");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.RoleType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt);

        builder.Property(r => r.DeletedAt);

        builder.Property(r => r.KeycloakSyncStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(KeycloakSyncStatus.Synced);

        builder.Property(r => r.LastSyncError)
            .HasMaxLength(500);

        builder.HasIndex(r => r.KeycloakSyncStatus)
            .HasFilter("\"KeycloakSyncStatus\" != 'Synced'");

        // Global soft delete filter
        builder.HasQueryFilter(r => r.DeletedAt == null);

        // Seed MTK system rolları
        SeedMtkRoles(builder);
    }

    private static void SeedMtkRoles(EntityTypeBuilder<Role> builder)
    {
        var now = new DateTime(2026, 9, 21, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            // SystemAdmin
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "SystemAdmin",
                Description = "Sistem administratoru - tam giriş hüququ",
                RoleType = RoleType.RealmRole,
                IsActive = true,
                KeycloakSyncStatus = KeycloakSyncStatus.PendingSync,
                CreatedAt = now
            },
            // BuildingManager (Komandant)
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "BuildingManager",
                Description = "Bina idarəçisi (Komandant) - bina əməliyyatları və maliyyəsini idarə edir",
                RoleType = RoleType.RealmRole,
                IsActive = true,
                KeycloakSyncStatus = KeycloakSyncStatus.PendingSync,
                CreatedAt = now
            },
            // Accountant (Mühasib)
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Accountant",
                Description = "Mühasib - maliyyə əməliyyatları və hesabatları idarə edir",
                RoleType = RoleType.RealmRole,
                IsActive = true,
                KeycloakSyncStatus = KeycloakSyncStatus.PendingSync,
                CreatedAt = now
            },
            // ApartmentOwner (Mənzil sahibi)
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "ApartmentOwner",
                Description = "Mənzil sahibi - bir və ya bir neçə mənzilin sahibi olan sakin",
                RoleType = RoleType.RealmRole,
                IsActive = true,
                KeycloakSyncStatus = KeycloakSyncStatus.PendingSync,
                CreatedAt = now
            },
            // Employee (İşçi)
            new
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Employee",
                Description = "İşçi - təmizlikçi, mühafizəçi, texniki işçilər",
                RoleType = RoleType.RealmRole,
                IsActive = true,
                KeycloakSyncStatus = KeycloakSyncStatus.PendingSync,
                CreatedAt = now
            }
        );
    }
}
