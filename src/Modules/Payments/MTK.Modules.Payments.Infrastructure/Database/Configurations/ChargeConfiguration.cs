using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class ChargeConfiguration : IEntityTypeConfiguration<Charge>
{
    public void Configure(EntityTypeBuilder<Charge> builder)
    {
        builder.ToTable("Charges");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.OwnerId)
            .IsRequired();

        builder.Property(c => c.PropertyType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.PropertyId)
            .IsRequired();

        builder.Property(c => c.Period)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.PaidAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        // Snapshot fields - calculation details at time of creation
        builder.Property(c => c.AreaSquareMeters)
            .HasPrecision(10, 2);

        builder.Property(c => c.RateAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.RateType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(c => c.DeletedAt == null);

        // Search vector for full-text search
        builder
            .HasGeneratedTsVectorColumn(
                c => c.SearchVector,
                "english",
                c => new { c.Period })
            .HasIndex(c => c.SearchVector)
            .HasMethod("GIN");

        // Indexes
        builder.HasIndex(c => c.OwnerId);
        builder.HasIndex(c => c.PropertyId);
        builder.HasIndex(c => c.Period);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => new { c.OwnerId, c.PropertyId, c.Period })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");
    }
}
