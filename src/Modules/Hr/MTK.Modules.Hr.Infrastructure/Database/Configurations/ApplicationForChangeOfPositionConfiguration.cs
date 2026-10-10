using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;

using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class ApplicationForChangeOfPositionConfiguration : IEntityTypeConfiguration<ApplicationForChangeOfPosition>
{
    public void Configure(EntityTypeBuilder<ApplicationForChangeOfPosition> builder)
    {
        builder.ToTable("application_for_change_of_position");

        builder.Property(a => a.EmployeeId)
            .IsRequired();

        builder.Property(a => a.CurrentJobId)
            .IsRequired();

        builder.Property(a => a.NewJobId)
            .IsRequired();

        builder.Property(a => a.SetDate)
            .IsRequired();

        builder.HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CurrentJob)
            .WithMany()
            .HasForeignKey(a => a.CurrentJobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.NewJob)
            .WithMany()
            .HasForeignKey(a => a.NewJobId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
