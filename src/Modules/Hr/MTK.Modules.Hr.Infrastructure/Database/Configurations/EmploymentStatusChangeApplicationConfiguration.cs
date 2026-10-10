using MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class EmploymentStatusChangeApplicationConfiguration : IEntityTypeConfiguration<EmploymentStatusChangeApplication>
{
    public void Configure(EntityTypeBuilder<EmploymentStatusChangeApplication> builder)
    {
        builder.ToTable("employment_status_change_applications");

        builder.Property(a => a.EmployeeId)
            .IsRequired();

        builder.HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.CurrentEmploymentType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.NewEmploymentType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.OrderExecutionSupervisorId)
            .IsRequired();

        builder.HasOne(a => a.OrderExecutionSupervisor)
            .WithMany()
            .HasForeignKey(a => a.OrderExecutionSupervisorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}