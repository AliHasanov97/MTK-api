using MTK.Modules.Hr.Domain.EducationLeaveOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class EducationLeaveOrderConfiguration : IEntityTypeConfiguration<EducationLeaveOrder>
{
    public void Configure(EntityTypeBuilder<EducationLeaveOrder> builder)
    {
        builder.ToTable("edu_leave_order");

        builder.Property(e => e.EducationLeaveApplicationId)
            .IsRequired();

        builder.HasOne(e => e.EducationLeaveApplication)
            .WithOne(ela => ela.EducationLeaveOrder)
            .HasForeignKey<EducationLeaveOrder>(e => e.EducationLeaveApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.EducationLeaveApplicationId)
            .IsUnique();

        builder.Property(e => e.EmployeeId)
            .IsRequired();

        builder.HasOne(e => e.Employee)
            .WithMany()
            .HasForeignKey(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.EmployeeId);

        builder.Property(e => e.Reason)
            .HasMaxLength(500);

        builder.Property(e => e.StartDate)
            .IsRequired();

        builder.Property(e => e.EndDate)
            .IsRequired();

        builder.Property(e => e.ReturnToWorkDate);

        builder.HasIndex(e => e.StartDate);
        builder.HasIndex(e => e.EndDate);
        builder.HasIndex(e => e.ReturnToWorkDate);
    }
}