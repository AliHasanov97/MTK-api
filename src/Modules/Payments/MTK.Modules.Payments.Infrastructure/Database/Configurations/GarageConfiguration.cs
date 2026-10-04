using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Garages;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class GarageConfiguration : IEntityTypeConfiguration<Garage>
{
    public void Configure(EntityTypeBuilder<Garage> builder)
    {
        builder.ToTable("garages");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.GarageNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(g => g.GarageType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);
    }
}
