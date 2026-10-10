using MTK.Modules.Hr.Domain.Employees;

using MTK.Modules.Hr.Domain.VacationCompensationApplications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class VacationCompensationApplicationConfiguration
    : IEntityTypeConfiguration<VacationCompensationApplication>
{
    public void Configure(EntityTypeBuilder<VacationCompensationApplication> builder)
    {
        builder.ToTable("vacation_compensation_applications");

        builder.Property(v => v.EmployeeId)
            .IsRequired();

        builder.HasOne(v => v.Employee)
            .WithMany()
            .HasForeignKey(v => v.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(v => v.RequestedDays)
            .IsRequired();

        builder.Property(v => v.Notes)
            .HasMaxLength(1000);

        // Index-lər
        builder.HasIndex(v => v.EmployeeId);
    }
}