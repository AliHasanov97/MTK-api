using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class PayrollBonusConfiguration : IEntityTypeConfiguration<PayrollBonus>
{
    public void Configure(EntityTypeBuilder<PayrollBonus> builder)
    {
        builder.ToTable("payroll_bonuses");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.EmployeeId).IsRequired();
        builder.Property(b => b.Amount).IsRequired().HasPrecision(18, 2);
        builder.Property(b => b.SalaryMonth).IsRequired();
        builder.Property(b => b.SalaryYear).IsRequired();

        builder.Property(b => b.CreatedAt).IsRequired();
        builder.Property(b => b.UpdatedAt);
        builder.Property(b => b.DeletedAt);

        builder.HasQueryFilter(b => b.DeletedAt == null);

        builder.HasIndex(b => new { b.EmployeeId, b.SalaryYear, b.SalaryMonth });
        builder.HasIndex(b => new { b.SalaryYear, b.SalaryMonth });
    }
}
