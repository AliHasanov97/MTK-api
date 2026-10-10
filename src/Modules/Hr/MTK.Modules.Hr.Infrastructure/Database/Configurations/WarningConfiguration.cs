using MTK.Modules.Hr.Domain.Warnings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class WarningOrderConfiguration : IEntityTypeConfiguration<WarningOrder>
{
    public void Configure(EntityTypeBuilder<WarningOrder> builder)
    {
        builder.ToTable("warnings");

        builder.Property(x => x.EmployeeId)
            .IsRequired();

        builder.Property(x => x.OrderExecutionSupervisorId)
            .IsRequired();

        builder.Property(x => x.SetDate)
            .IsRequired();

        builder.Property(x => x.DisciplinaryType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(DisciplinaryType.Warning); 
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
