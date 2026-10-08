using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Purchases;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("Purchases");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.VendorId)
            .IsRequired();

        builder.Property(p => p.CreatedByUserId);
        builder.Property(p => p.ReceivedByUserId);

        builder.Property(p => p.PurchaseDate)
            .IsRequired();

        builder.Property(p => p.InvoiceNumber)
            .HasMaxLength(100);

        builder.Property(p => p.Note)
            .HasMaxLength(1000);

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.ReceivedOnUtc);

        builder.Property(p => p.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt);
        builder.Property(p => p.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(p => p.DeletedAt == null);

        // Search vector for full-text search
        builder
            .HasGeneratedTsVectorColumn(
                p => p.SearchVector,
                "english",
                p => new { p.InvoiceNumber, p.Note })
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN");

        builder.HasIndex(p => p.VendorId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.PurchaseDate);

        // Sətirlər bu aggregate-in içindədir → real FK + cascade.
        builder
            .HasMany(p => p.Lines)
            .WithOne()
            .HasForeignKey(l => l.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
