using MTK.Modules.Hr.Domain.CompensationOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class CompensationOrderConfiguration : IEntityTypeConfiguration<CompensationOrder>
{
    public void Configure(EntityTypeBuilder<CompensationOrder> builder)
    {
        builder.ToTable("compensation_orders");

        builder.Property(c => c.CompensationApplicationId)
            .IsRequired();

        builder.HasOne(c => c.CompensationApplication)
            .WithOne(ca => ca.CompensationOrder)
            .HasForeignKey<CompensationOrder>(c => c.CompensationApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.CompensationApplicationId)
            .IsUnique();

        builder.Property(c => c.EmployeeId)
            .IsRequired();

        builder.HasOne(c => c.Employee)
            .WithMany()
            .HasForeignKey(c => c.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.EmployeeId);

        builder.Property(c => c.CompensatedDays)
            .IsRequired();

        builder.Property(c => c.Notes)
            .HasMaxLength(1000);
    }
}
