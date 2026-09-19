using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Identity.Domain.Groups;
using MTK.Modules.Identity.Domain.Keycloak;

namespace MTK.Modules.Identity.Infrastructure.Database.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups", "identity");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.KeycloakGroupId)
            .IsRequired();

        builder.HasIndex(g => g.KeycloakGroupId)
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(g => g.Description)
            .HasMaxLength(500);

        builder.Property(g => g.ParentGroupId);

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        builder.Property(g => g.UpdatedAt);

        builder.Property(g => g.DeletedAt);

        builder.Property(g => g.KeycloakSyncStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(KeycloakSyncStatus.Synced);

        builder.Property(g => g.LastSyncError)
            .HasMaxLength(500);

        builder.HasIndex(g => g.KeycloakSyncStatus)
            .HasFilter("\"KeycloakSyncStatus\" != 'Synced'");

        // Global soft delete filter
        builder.HasQueryFilter(g => g.DeletedAt == null);

        // Self-referencing relationship for hierarchy
        builder.HasOne<Group>()
            .WithMany()
            .HasForeignKey(g => g.ParentGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
