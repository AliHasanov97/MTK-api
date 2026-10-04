using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Apartments;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.ToTable("apartments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ApartmentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.AreaSquareMeters)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.HasOne(a => a.Building)
            .WithMany()
            .HasForeignKey(a => a.BuildingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.BuildingId);
    }
}
