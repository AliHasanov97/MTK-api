using MTK.Modules.Hr.Domain.BonusOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class BonusOrderConfiguration : IEntityTypeConfiguration<BonusOrder>
{
    public void Configure(EntityTypeBuilder<BonusOrder> builder)
    {
        builder.ToTable("bonus_orders");

        builder.Property(x => x.EmployeeId)
            .IsRequired();

        builder.Property(x => x.OrderExecutionSupervisorId)
            .IsRequired();

        builder.Property(x => x.BonusQuantity)
            .IsRequired();

        builder.Property(x => x.SalaryMonth)
            .IsRequired();

        builder.Property(x => x.SalaryYear)
            .IsRequired();

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OrderExecutionSupervisor)
            .WithMany()
            .HasForeignKey(x => x.OrderExecutionSupervisorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
