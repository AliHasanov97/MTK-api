using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.PropertyOwnerships;

namespace MTK.Modules.Payments.Infrastructure.PropertyOwnerships;

internal sealed class PropertyOwnershipConfiguration : IEntityTypeConfiguration<PropertyOwnership>
{
    public void Configure(EntityTypeBuilder<PropertyOwnership> builder)
    {
        builder.ToTable("property_ownerships");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PropertyId)
            .IsRequired();

        builder.Property(p => p.PropertyType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.OwnerId)
            .IsRequired();

        builder.Property(p => p.AreaSquareMeters)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.GarageType)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Unique index on PropertyId to ensure one entry per property
        builder.HasIndex(p => p.PropertyId)
            .IsUnique();

        // Index for faster queries
        builder.HasIndex(p => p.OwnerId);
        builder.HasIndex(p => p.PropertyType);
    }
}
