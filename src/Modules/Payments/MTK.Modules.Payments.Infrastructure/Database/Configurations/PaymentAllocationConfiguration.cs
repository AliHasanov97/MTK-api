using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class PaymentAllocationConfiguration : IEntityTypeConfiguration<PaymentAllocation>
{
    public void Configure(EntityTypeBuilder<PaymentAllocation> builder)
    {
        builder.ToTable("PaymentAllocations");

        builder.HasKey(pa => pa.Id);

        builder.Property(pa => pa.PaymentId)
            .IsRequired();

        builder.Property(pa => pa.ChargeId)
            .IsRequired();

        builder.Property(pa => pa.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(pa => pa.CreatedAt)
            .IsRequired();

        builder.Property(pa => pa.UpdatedAt);

        builder.Property(pa => pa.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(pa => pa.DeletedAt == null);

        // Indexes
        builder.HasIndex(pa => pa.PaymentId);
        builder.HasIndex(pa => pa.ChargeId);
    }
}
