using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Rates;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class RateConfiguration : IEntityTypeConfiguration<Rate>
{
    public void Configure(EntityTypeBuilder<Rate> builder)
    {
        builder.ToTable("Rates");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.RateType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(r => r.EffectiveFrom)
            .IsRequired();

        builder.Property(r => r.EffectiveTo);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.GarageType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt);

        builder.Property(r => r.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(r => r.DeletedAt == null);

        // Search vector for full-text search
        builder
            .HasGeneratedTsVectorColumn(
                r => r.SearchVector,
                "english",
                r => new { r.Description })
            .HasIndex(r => r.SearchVector)
            .HasMethod("GIN");

        // Indexes
        builder.HasIndex(r => new { r.RateType, r.EffectiveFrom });
        builder.HasIndex(r => new { r.RateType, r.GarageType, r.EffectiveFrom });
        builder.HasIndex(r => r.EffectiveTo);
    }
}
