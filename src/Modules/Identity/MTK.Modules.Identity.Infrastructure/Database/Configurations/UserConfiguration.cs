using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Infrastructure.Database.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.IdentityId)
            .HasMaxLength(100);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(u => u.KeycloakSyncStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(u => u.LastSyncError)
            .HasMaxLength(500);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.DeletedAt);

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("[DeletedAt] IS NULL");

        builder.HasIndex(u => u.IdentityId)
            .HasFilter("[IdentityId] IS NOT NULL");

        builder.HasIndex(u => u.KeycloakSyncStatus)
            .HasFilter("[KeycloakSyncStatus] != 'Synced'");

        // Ignore domain events
        builder.Ignore(u => u.DomainEvents);
    }
}
