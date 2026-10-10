using MTK.Modules.Hr.Domain.VacationApplications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class VacationApplicationConfiguration
    : IEntityTypeConfiguration<VacationApplication>
{
    public void Configure(EntityTypeBuilder<VacationApplication> builder)
    {
        builder.ToTable("vacation_applications");

        builder.Property(v => v.EmployeeId)
            .IsRequired();

        builder.HasOne(v => v.Employee)
            .WithMany()
            .HasForeignKey(v => v.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(v => v.StartDate)
            .IsRequired();

        builder.Property(v => v.EndDate)
            .IsRequired(false);  // Nullable: əgər RequestedDays göndərilərsə handler hesablayır

        builder.Property(v => v.TotalRequestedDays)
            .IsRequired();

        builder.Property(v => v.Notes)
            .HasMaxLength(1000);

        // Index-lər
        builder.HasIndex(v => v.EmployeeId);
        builder.HasIndex(v => v.StartDate);
        builder.HasIndex(v => v.EndDate);
    }
}