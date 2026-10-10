using MTK.Modules.Hr.Domain.VacationOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class VacationOrderConfiguration : IEntityTypeConfiguration<VacationOrder>
{
    public void Configure(EntityTypeBuilder<VacationOrder> builder)
    {
        builder.ToTable("vacation_orders");

        builder.Property(v => v.VacationApplicationId)
            .IsRequired();

        builder.HasOne(v => v.VacationApplication)
            .WithOne(va => va.VacationOrder)
            .HasForeignKey<VacationOrder>(v => v.VacationApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => v.VacationApplicationId)
            .IsUnique();

        builder.Property(v => v.EmployeeId)
            .IsRequired();

        builder.HasOne(v => v.Employee)
            .WithMany()
            .HasForeignKey(v => v.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => v.EmployeeId);

        builder.Property(v => v.StartDate)
            .IsRequired();

        builder.Property(v => v.EndDate)
            .IsRequired();

        builder.Property(v => v.VacationDays)
            .IsRequired();

        builder.Property(v => v.ReturnToWorkDate);

        builder.Property(v => v.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(v => v.StartDate);
        builder.HasIndex(v => v.EndDate);
        builder.HasIndex(v => v.ReturnToWorkDate);
    }
}
