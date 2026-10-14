using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class PayrollRunConfiguration : IEntityTypeConfiguration<PayrollRun>
{
    public void Configure(EntityTypeBuilder<PayrollRun> builder)
    {
        builder.ToTable("payroll_runs");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Year)
            .IsRequired();

        builder.Property(r => r.Month)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.CalculatedAt);
        builder.Property(r => r.ApprovedAt);
        builder.Property(r => r.PaidAt);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt);

        builder.Property(r => r.DeletedAt);

        builder.HasQueryFilter(r => r.DeletedAt == null);

        // Computed-only properties — EF-ə naviqasiya kimi görünməsin.
        builder.Ignore(r => r.TotalGross);
        builder.Ignore(r => r.TotalNet);
        builder.Ignore(r => r.TotalEmployerCost);

        builder.HasIndex(r => new { r.Year, r.Month })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        // Payslip-lər bu aggregate-in daxilindədir → real FK + cascade.
        builder
            .HasMany(r => r.Payslips)
            .WithOne()
            .HasForeignKey(p => p.PayrollRunId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
