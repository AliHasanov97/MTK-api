using MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class EmploymentStatusChangeOrderConfiguration : IEntityTypeConfiguration<EmploymentStatusChangeOrder>
{
    public void Configure(EntityTypeBuilder<EmploymentStatusChangeOrder> builder)
    {
        builder.ToTable("employment_status_change_orders");

        builder.Property(o => o.EmploymentStatusChangeApplicationId)
            .IsRequired();

        builder.HasOne(o => o.EmploymentStatusChangeApplication)
            .WithOne(a => a.EmploymentStatusChangeOrder)
            .HasForeignKey<EmploymentStatusChangeOrder>(o => o.EmploymentStatusChangeApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.EmploymentStatusChangeApplicationId)
            .IsUnique();

        builder.Property(o => o.EmployeeId)
            .IsRequired();

        builder.HasOne(o => o.Employee)
            .WithMany()
            .HasForeignKey(o => o.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(o => o.CurrentEmploymentType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(o => o.NewEmploymentType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(o => o.OrderExecutionSupervisorId)
            .IsRequired();

        builder.HasOne(o => o.OrderExecutionSupervisor)
            .WithMany()
            .HasForeignKey(o => o.OrderExecutionSupervisorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}