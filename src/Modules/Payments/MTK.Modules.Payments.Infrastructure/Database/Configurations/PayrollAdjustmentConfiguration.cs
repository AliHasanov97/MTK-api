using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class PayrollAdjustmentConfiguration : IEntityTypeConfiguration<PayrollAdjustment>
{
    public void Configure(EntityTypeBuilder<PayrollAdjustment> builder)
    {
        builder.ToTable("payroll_adjustments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.EmployeeId).IsRequired();

        builder.Property(a => a.Reason)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.Hours).IsRequired().HasPrecision(18, 2);
        builder.Property(a => a.NormHours).IsRequired().HasPrecision(18, 2);
        builder.Property(a => a.SalaryMonth).IsRequired();
        builder.Property(a => a.SalaryYear).IsRequired();

        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt);
        builder.Property(a => a.DeletedAt);

        builder.HasQueryFilter(a => a.DeletedAt == null);

        builder.HasIndex(a => new { a.EmployeeId, a.SalaryYear, a.SalaryMonth });
        builder.HasIndex(a => new { a.SalaryYear, a.SalaryMonth });
    }
}
