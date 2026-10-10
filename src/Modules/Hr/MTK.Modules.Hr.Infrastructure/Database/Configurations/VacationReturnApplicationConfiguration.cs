using MTK.Modules.Hr.Domain.VacationReturnApplications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class VacationReturnApplicationConfiguration
    : IEntityTypeConfiguration<VacationReturnApplication>
{
    public void Configure(EntityTypeBuilder<VacationReturnApplication> builder)
    {
        builder.ToTable("vacation_return_applications");

        builder.Property(v => v.EmployeeId)
            .IsRequired();

        builder.HasOne(v => v.Employee)
            .WithMany()
            .HasForeignKey(v => v.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(v => v.ReturnDate)
            .IsRequired();

        builder.Property(v => v.Notes)
            .HasMaxLength(1000);

        // Index-lər
        builder.HasIndex(v => v.EmployeeId);
        builder.HasIndex(v => v.ReturnDate);
    }
}
