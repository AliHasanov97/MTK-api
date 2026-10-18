using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.VacationPays;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class VacationPayConfiguration : IEntityTypeConfiguration<VacationPay>
{
    public void Configure(EntityTypeBuilder<VacationPay> builder)
    {
        builder.ToTable("vacation_pays");

        builder.HasKey(vp => vp.Id);

        builder.Property(vp => vp.VacationOrderId)
            .IsRequired();

        builder.Property(vp => vp.EmployeeId)
            .IsRequired();

        builder.Property(vp => vp.EmployeeFullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(vp => vp.AverageMonthlyWage)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(vp => vp.AverageDailyWage)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(vp => vp.VacationDays)
            .IsRequired();

        builder.Property(vp => vp.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(vp => vp.VacationStartDate)
            .IsRequired();

        builder.Property(vp => vp.VacationEndDate)
            .IsRequired();

        builder.Property(vp => vp.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(vp => vp.DueDate)
            .IsRequired();

        builder.Property(vp => vp.PaidAt);

        builder.Property(vp => vp.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(vp => vp.CreatedAt)
            .IsRequired();

        builder.Property(vp => vp.UpdatedAt);

        builder.Property(vp => vp.DeletedAt);

        // Indexes
        builder.HasIndex(vp => vp.VacationOrderId)
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        builder.HasIndex(vp => vp.EmployeeId);

        builder.HasIndex(vp => vp.DueDate);

        builder.HasIndex(vp => vp.Status);

        builder.HasIndex(vp => new { vp.Status, vp.DueDate });

        // Soft delete filter
        builder.HasQueryFilter(vp => vp.DeletedAt == null);
    }
}
