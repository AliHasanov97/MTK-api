using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Buildings.Domain.Buildings;

namespace MTK.Modules.Buildings.Infrastructure.Database.Configurations;

internal sealed class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.ToTable("Buildings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(b => b.Name)
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        // Address value object
        builder.OwnsOne(b => b.Address, address =>
        {
            address.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("Street");

            address.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("City");

            address.Property(a => a.District)
                .HasMaxLength(100)
                .HasColumnName("District");

            address.Property(a => a.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("PostalCode");

            address.Property(a => a.Country)
                .HasMaxLength(100)
                .HasColumnName("Country");
        });

        builder.Property(b => b.TotalFloors)
            .IsRequired();

        builder.Property(b => b.ApartmentsPerFloor)
            .IsRequired();

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Description)
            .HasMaxLength(500);

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt);

        builder.Property(b => b.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(b => b.DeletedAt == null);

        // Relationships
        builder.HasMany(b => b.Apartments)
            .WithOne(a => a.Building)
            .HasForeignKey(a => a.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
