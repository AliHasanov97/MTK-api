using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.VendorCharges;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class VendorChargeConfiguration : IEntityTypeConfiguration<VendorCharge>
{
    public void Configure(EntityTypeBuilder<VendorCharge> builder)
    {
        builder.ToTable("VendorCharges");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ContractId)
            .IsRequired();

        builder.Property(c => c.VendorId)
            .IsRequired();

        builder.Property(c => c.ContractServiceId);

        builder.Property(c => c.ContractGoodsItemId);

        builder.Property(c => c.Period)
            .HasMaxLength(7);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.Reference)
            .HasMaxLength(200);

        builder.Property(c => c.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.Quantity)
            .IsRequired()
            .HasPrecision(18, 2);

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

        builder.Property(c => c.Source)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.ChargeDate)
            .IsRequired();

        builder.Property(c => c.DueDate);

        builder.Property(c => c.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(c => c.CancellationReason)
            .HasMaxLength(1000);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.DeletedAt);

        // Hesablanan sahələr bazada saxlanılmır.
        builder.Ignore(c => c.OutstandingAmount);
        builder.Ignore(c => c.IsOverdue);

        // Global soft delete filter
        builder.HasQueryFilter(c => c.DeletedAt == null);

        // Search vector for full-text search
        builder
            .HasGeneratedTsVectorColumn(
                c => c.SearchVector,
                "english",
                c => new { c.Description, c.Reference })
            .HasIndex(c => c.SearchVector)
            .HasMethod("GIN");

        // Indexes
        builder.HasIndex(c => c.ContractId);
        builder.HasIndex(c => c.VendorId);
        builder.HasIndex(c => c.ContractServiceId);
        builder.HasIndex(c => c.ContractGoodsItemId);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.Source);
        builder.HasIndex(c => c.ChargeDate);
        builder.HasIndex(c => c.DueDate);

        // İdempotentlik: eyni xidmət üçün eyni dövrə, eyni mal üçün eyni qaiməyə
        // ikinci borc yaranmasın. Job yenidən işləsə də təkrar sətir yazılmır.
        builder.HasIndex(c => new { c.ContractServiceId, c.Period })
            .IsUnique()
            .HasFilter("\"ContractServiceId\" IS NOT NULL AND \"DeletedAt\" IS NULL");

        builder.HasIndex(c => new { c.ContractGoodsItemId, c.Reference })
            .IsUnique()
            .HasFilter("\"ContractGoodsItemId\" IS NOT NULL AND \"Reference\" IS NOT NULL AND \"DeletedAt\" IS NULL");
    }
}
