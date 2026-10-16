using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.SalaryPayments;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class SalaryPaymentLineConfiguration : IEntityTypeConfiguration<SalaryPaymentLine>
{
    public void Configure(EntityTypeBuilder<SalaryPaymentLine> builder)
    {
        builder.ToTable("salary_payment_lines");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.SalaryPaymentId)
            .IsRequired();

        builder.Property(l => l.LineType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(l => l.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(l => l.SourceBonusId);

        builder.Property(l => l.SourceAdjustmentId);

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.Property(l => l.UpdatedAt);

        builder.Property(l => l.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(l => l.DeletedAt == null);

        // Indexes
        builder.HasIndex(l => l.SalaryPaymentId);
    }
}
