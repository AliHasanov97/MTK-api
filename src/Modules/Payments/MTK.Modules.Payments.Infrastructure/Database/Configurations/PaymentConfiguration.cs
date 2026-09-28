using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.OwnerId)
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.PaymentMethod)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.PaymentDate)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Reference)
            .HasMaxLength(200);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(p => p.DeletedAt == null);

        // Search vector for full-text search
        builder
            .HasGeneratedTsVectorColumn(
                p => p.SearchVector,
                "english",
                p => new { p.Reference, p.Notes })
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN");

        // Indexes
        builder.HasIndex(p => p.OwnerId);
        builder.HasIndex(p => p.PaymentDate);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.Reference);
    }
}
