using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class PayrollDeductionConfiguration : IEntityTypeConfiguration<PayrollDeduction>
{
    public void Configure(EntityTypeBuilder<PayrollDeduction> builder)
    {
        builder.ToTable("payroll_deductions");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.EmployeeId).IsRequired();
        builder.Property(d => d.PercentageSalary).IsRequired().HasPrecision(5, 2);
        builder.Property(d => d.StartDate).IsRequired();

        builder.Property(d => d.CreatedAt).IsRequired();
        builder.Property(d => d.UpdatedAt);
        builder.Property(d => d.DeletedAt);

        builder.HasQueryFilter(d => d.DeletedAt == null);

        builder.HasIndex(d => d.EmployeeId);
        builder.HasIndex(d => d.StartDate);
    }
}
