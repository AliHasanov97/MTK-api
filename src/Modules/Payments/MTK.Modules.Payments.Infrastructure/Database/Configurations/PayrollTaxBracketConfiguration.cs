using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class PayrollTaxBracketConfiguration : IEntityTypeConfiguration<PayrollTaxBracket>
{
    public void Configure(EntityTypeBuilder<PayrollTaxBracket> builder)
    {
        builder.ToTable("payroll_tax_brackets");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.RateType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Party)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.LowerBound)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(b => b.UpperBound)
            .HasPrecision(18, 2);

        builder.Property(b => b.Rate)
            .IsRequired()
            .HasPrecision(5, 4);

        builder.Property(b => b.EffectiveFrom)
            .IsRequired();

        builder.Property(b => b.EffectiveTo);

        builder.Property(b => b.Description)
            .HasMaxLength(500);

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt);

        builder.Property(b => b.DeletedAt);

        builder.HasQueryFilter(b => b.DeletedAt == null);

        builder.HasIndex(b => new { b.RateType, b.Party, b.EffectiveFrom });
        builder.HasIndex(b => b.EffectiveTo);
    }
}
