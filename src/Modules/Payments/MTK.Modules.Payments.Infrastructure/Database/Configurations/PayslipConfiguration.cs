using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class PayslipConfiguration : IEntityTypeConfiguration<Payslip>
{
    public void Configure(EntityTypeBuilder<Payslip> builder)
    {
        builder.ToTable("payslips");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PayrollRunId)
            .IsRequired();

        builder.Property(p => p.EmployeeId)
            .IsRequired();

        builder.Property(p => p.EmployeeFullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.GrossSalary).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.EmployeeDsmf).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.EmployeeUnemployment).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.EmployeeMedicalInsurance).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.TaxableIncome).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.IncomeTax).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.NetSalary).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.EmployerDsmf).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.EmployerUnemployment).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.EmployerMedicalInsurance).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.TotalEmployerCost).IsRequired().HasPrecision(18, 2);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.DeletedAt);

        builder.HasQueryFilter(p => p.DeletedAt == null);

        builder.HasIndex(p => p.PayrollRunId);
        builder.HasIndex(p => p.EmployeeId);
        builder.HasIndex(p => new { p.PayrollRunId, p.EmployeeId }).IsUnique();
    }
}
