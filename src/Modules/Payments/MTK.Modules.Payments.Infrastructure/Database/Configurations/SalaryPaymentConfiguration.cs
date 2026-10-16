using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.SalaryPayments;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class SalaryPaymentConfiguration : IEntityTypeConfiguration<SalaryPayment>
{
    public void Configure(EntityTypeBuilder<SalaryPayment> builder)
    {
        builder.ToTable("salary_payments");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.EmployeeId)
            .IsRequired();

        builder.Property(s => s.EmployeeFullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.SalaryMonth)
            .IsRequired();

        builder.Property(s => s.SalaryYear)
            .IsRequired();

        builder.Property(s => s.BaseSalary)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.TotalBonuses)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.TotalAdjustments)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.GrossSalary)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.PaymentMethod)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.PaymentDate)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        builder.Property(s => s.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(s => s.DeletedAt == null);

        // One-to-many with lines (cascade delete)
        builder.HasMany(s => s.Lines)
            .WithOne()
            .HasForeignKey(l => l.SalaryPaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Search vector for full-text search
        builder
            .HasGeneratedTsVectorColumn(
                s => s.SearchVector,
                "simple",
                s => new { s.EmployeeFullName, s.Notes })
            .HasIndex(s => s.SearchVector)
            .HasMethod("GIN");

        // Indexes
        builder.HasIndex(s => s.EmployeeId);
        builder.HasIndex(s => new { s.SalaryYear, s.SalaryMonth });
        builder.HasIndex(s => s.Status);

        // Unique constraint: one payment per employee per period
        builder.HasIndex(s => new { s.EmployeeId, s.SalaryYear, s.SalaryMonth })
            .IsUnique()
            .HasFilter("deleted_at IS NULL");
    }
}
