using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Buildings.Domain.ApartmentTypes;
using MTK.Modules.Buildings.Domain.Buildings;

namespace MTK.Modules.Buildings.Infrastructure.Database.Configurations;

internal sealed class ApartmentTypeConfiguration : IEntityTypeConfiguration<ApartmentType>
{
    public void Configure(EntityTypeBuilder<ApartmentType> builder)
    {
        builder.ToTable("ApartmentTypes");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.BuildingId)
            .IsRequired();

        builder.Property(t => t.RoomCount)
            .IsRequired();

        builder.Property(t => t.AreaSquareMeters)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.DeletedAt);

        builder.HasQueryFilter(t => t.DeletedAt == null);

        builder.HasOne<Building>()
            .WithMany()
            .HasForeignKey(t => t.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
