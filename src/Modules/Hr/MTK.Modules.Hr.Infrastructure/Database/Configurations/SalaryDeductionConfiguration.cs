using MTK.Modules.Hr.Domain.SalaryDeductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class SalaryDeductionConfiguration : IEntityTypeConfiguration<SalaryDeduction>
{
    public void Configure(EntityTypeBuilder<SalaryDeduction> builder)
    {
        builder.ToTable("salary_deductions");

        builder.Property(x => x.EmployeeId)
            .IsRequired();

        builder.Property(x => x.District)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.JudgementNo)
            .IsRequired();

        builder.Property(x => x.JudgementDate)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.PercentageSalary)
            .IsRequired();

        builder.Property(x => x.StateFee)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Creditor)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Debt)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}