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
    }
}
