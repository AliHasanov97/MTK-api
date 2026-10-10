using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EducationLeaveApplications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class EducationLeaveApplicationConfiguration
    : IEntityTypeConfiguration<EducationLeaveApplication>
{
    public void Configure(EntityTypeBuilder<EducationLeaveApplication> builder)
    {
        builder.ToTable("edu_leave_app");

        builder.Property(e => e.EmployeeId)
            .IsRequired();

        builder.HasOne(e => e.Employee)
            .WithMany()
            .HasForeignKey(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.Reason)
            .HasMaxLength(500);

        builder.Property(e => e.StartDate)
            .IsRequired();

        builder.Property(e => e.EndDate)
            .IsRequired();

        // Index-lər
        builder.HasIndex(e => e.EmployeeId);
        builder.HasIndex(e => e.StartDate);
        builder.HasIndex(e => e.EndDate);
    }
}