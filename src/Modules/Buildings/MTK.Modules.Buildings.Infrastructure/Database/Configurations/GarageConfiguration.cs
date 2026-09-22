using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Buildings.Domain.Garages;

namespace MTK.Modules.Buildings.Infrastructure.Database.Configurations;

internal sealed class GarageConfiguration : IEntityTypeConfiguration<Garage>
{
    public void Configure(EntityTypeBuilder<Garage> builder)
    {
        builder.ToTable("Garages");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.ApartmentId)
            .IsRequired();

        builder.Property(g => g.GarageNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(g => g.GarageNumber)
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        builder.Property(g => g.Type)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(g => g.OwnerId);

        builder.Property(g => g.Description)
            .HasMaxLength(500);

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        builder.Property(g => g.UpdatedAt);

        builder.Property(g => g.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(g => g.DeletedAt == null);

        // Relationships
        builder.HasOne(g => g.Apartment)
            .WithMany(a => a.Garages)
            .HasForeignKey(g => g.ApartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.Owner)
            .WithMany(o => o.OwnedGarages)
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
