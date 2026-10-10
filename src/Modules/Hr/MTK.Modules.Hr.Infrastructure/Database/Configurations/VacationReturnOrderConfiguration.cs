using MTK.Modules.Hr.Domain.VacationReturnOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class VacationReturnOrderConfiguration : IEntityTypeConfiguration<VacationReturnOrder>
{
    public void Configure(EntityTypeBuilder<VacationReturnOrder> builder)
    {
        builder.ToTable("vacation_return_orders");

        builder.Property(v => v.VacationReturnApplicationId)
            .IsRequired();

        builder.HasOne(v => v.VacationReturnApplication)
            .WithOne(va => va.VacationReturnOrder)
            .HasForeignKey<VacationReturnOrder>(v => v.VacationReturnApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => v.VacationReturnApplicationId)
            .IsUnique();

        builder.Property(v => v.EmployeeId)
            .IsRequired();

        builder.HasOne(v => v.Employee)
            .WithMany()
            .HasForeignKey(v => v.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => v.EmployeeId);

        builder.Property(v => v.ReturnDate)
            .IsRequired();

        builder.Property(v => v.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(v => v.ReturnDate);
    }
}
