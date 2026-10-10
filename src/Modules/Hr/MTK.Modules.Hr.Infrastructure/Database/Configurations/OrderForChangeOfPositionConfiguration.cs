using MTK.Modules.Hr.Domain.OrdersForChangeOfPosition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class OrderForChangeOfPositionConfiguration : IEntityTypeConfiguration<OrderForChangeOfPosition>
{
    public void Configure(EntityTypeBuilder<OrderForChangeOfPosition> builder)
    {
        builder.ToTable("order_for_change_of_position");

        builder.Property(o => o.ApplicationForChangeOfPositionId)
            .IsRequired();

        builder.HasOne(o => o.ApplicationForChangeOfPosition)
            .WithOne(a => a.OrderForChangeOfPosition)
            .HasForeignKey<OrderForChangeOfPosition>(o => o.ApplicationForChangeOfPositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.ApplicationForChangeOfPositionId)
            .IsUnique();

        builder.Property(o => o.EmployeeId)
            .IsRequired();

        builder.HasOne(o => o.Employee)
            .WithMany()
            .HasForeignKey(o => o.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
