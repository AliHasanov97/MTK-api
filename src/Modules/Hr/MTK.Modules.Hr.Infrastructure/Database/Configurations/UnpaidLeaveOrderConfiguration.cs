using MTK.Modules.Hr.Domain.UnpaidLeaveOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class UnpaidLeaveOrderConfiguration : IEntityTypeConfiguration<UnpaidLeaveOrder>
{
    public void Configure(EntityTypeBuilder<UnpaidLeaveOrder> builder)
    {
        builder.ToTable("unpaid_leave_orders");

        builder.Property(u => u.UnpaidLeaveApplicationId)
            .IsRequired();

        builder.HasOne(u => u.UnpaidLeaveApplication)
            .WithOne(ula => ula.UnpaidLeaveOrder)
            .HasForeignKey<UnpaidLeaveOrder>(u => u.UnpaidLeaveApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.UnpaidLeaveApplicationId)
            .IsUnique();

        builder.Property(u => u.EmployeeId)
            .IsRequired();

        builder.HasOne(u => u.Employee)
            .WithMany()
            .HasForeignKey(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.EmployeeId);

        builder.Property(u => u.StartDate)
            .IsRequired();

        builder.Property(u => u.EndDate)
            .IsRequired();

        builder.Property(u => u.ReturnToWorkDate);

        builder.Property(u => u.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(u => u.StartDate);
        builder.HasIndex(u => u.EndDate);
        builder.HasIndex(u => u.ReturnToWorkDate);
    }
}
