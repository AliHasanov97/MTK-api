using MTK.Modules.Hr.Domain.EmployeeWorkSchedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class EmployeeWorkScheduleConfiguration : IEntityTypeConfiguration<EmployeeWorkSchedule>
{
    public void Configure(EntityTypeBuilder<EmployeeWorkSchedule> builder)
    {
        builder.ToTable("employee_work_schedules");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EmployeeId)
            .IsRequired();

        builder.Property(e => e.EffectiveFrom)
            .IsRequired();

        builder.Property(e => e.Monday).HasPrecision(4, 2);
        builder.Property(e => e.Tuesday).HasPrecision(4, 2);
        builder.Property(e => e.Wednesday).HasPrecision(4, 2);
        builder.Property(e => e.Thursday).HasPrecision(4, 2);
        builder.Property(e => e.Friday).HasPrecision(4, 2);
        builder.Property(e => e.Saturday).HasPrecision(4, 2);
        builder.Property(e => e.Sunday).HasPrecision(4, 2);

        builder.HasOne(e => e.Employee)
            .WithMany(e => e.WorkSchedules)
            .HasForeignKey(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Eyni işçi üçün eyni tarixdə iki qrafik ola bilməz
        builder.HasIndex(e => new { e.EmployeeId, e.EffectiveFrom }).IsUnique();
    }
}
