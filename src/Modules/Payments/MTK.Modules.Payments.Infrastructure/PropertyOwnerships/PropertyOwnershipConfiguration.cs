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

        builder.Property(p => p.ApartmentId);
        builder.Property(p => p.GarageId);

        builder.Property(p => p.OwnerId)
            .IsRequired();

        // Bir əmlak üçün bir mülkiyyət qeydi — ApartmentId/GarageId qarşılıqlı
        // müstəsna olduğu üçün iki ayrı filtrlənmiş unikal indeks kifayətdir.
        builder.HasIndex(p => p.ApartmentId)
            .IsUnique()
            .HasFilter("\"ApartmentId\" IS NOT NULL");
        builder.HasIndex(p => p.GarageId)
            .IsUnique()
            .HasFilter("\"GarageId\" IS NOT NULL");

        builder.HasIndex(p => p.OwnerId);

        // Naviqasiyalar — Restrict: shadow silinsə belə mülkiyyət tarixçəsi qalmalıdır.
        builder.HasOne(p => p.Apartment)
            .WithMany()
            .HasForeignKey(p => p.ApartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Garage)
            .WithMany()
            .HasForeignKey(p => p.GarageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
