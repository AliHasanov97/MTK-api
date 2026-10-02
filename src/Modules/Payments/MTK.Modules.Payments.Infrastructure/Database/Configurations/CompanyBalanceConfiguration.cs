using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.CompanyBalances;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class CompanyBalanceConfiguration : IEntityTypeConfiguration<CompanyBalance>
{
    public void Configure(EntityTypeBuilder<CompanyBalance> builder)
    {
        builder.ToTable("CompanyBalances");

        builder.HasKey(cb => cb.Id);

        builder.Property(cb => cb.TotalIncome)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(cb => cb.TotalExpense)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(cb => cb.CreatedAt)
            .IsRequired();

        builder.Property(cb => cb.UpdatedAt);
    }
}
