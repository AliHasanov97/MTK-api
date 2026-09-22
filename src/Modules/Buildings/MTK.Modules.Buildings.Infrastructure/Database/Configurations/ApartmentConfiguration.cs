using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Buildings.Domain.Apartments;

namespace MTK.Modules.Buildings.Infrastructure.Database.Configurations;

internal sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.ToTable("Apartments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.BuildingId)
            .IsRequired();

        builder.Property(a => a.ApartmentNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(a => new { a.BuildingId, a.ApartmentNumber })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        builder.Property(a => a.Floor)
            .IsRequired();

        builder.Property(a => a.AreaSquareMeters)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(a => a.RoomCount)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.CurrentOwnerId);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt);

        builder.Property(a => a.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(a => a.DeletedAt == null);

        // Relationships
        builder.HasOne(a => a.Building)
            .WithMany(b => b.Apartments)
            .HasForeignKey(a => a.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CurrentOwner)
            .WithMany(o => o.OwnedApartments)
            .HasForeignKey(a => a.CurrentOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Garages)
            .WithOne(g => g.Apartment)
            .HasForeignKey(g => g.ApartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
