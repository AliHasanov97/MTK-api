using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class UnpaidLeaveApplicationConfiguration
    : IEntityTypeConfiguration<UnpaidLeaveApplication>
{
    public void Configure(EntityTypeBuilder<UnpaidLeaveApplication> builder)
    {
        builder.ToTable("unpaid_leave_applications");

        builder.Property(u => u.EmployeeId)
            .IsRequired();

        builder.HasOne(u => u.Employee)
            .WithMany()
            .HasForeignKey(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(u => u.StartDate)
            .IsRequired();

        builder.Property(u => u.EndDate)
            .IsRequired();

        builder.Property(u => u.Notes)
            .HasMaxLength(1000);

        // Index-lər
        builder.HasIndex(u => u.EmployeeId);
        builder.HasIndex(u => u.StartDate);
        builder.HasIndex(u => u.EndDate);
    }
}
