using MTK.Modules.Hr.Domain.UnexcusedAbsences;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class UnexcusedAbsenceOrderConfiguration : IEntityTypeConfiguration<UnexcusedAbsenceOrder>
{
    public void Configure(EntityTypeBuilder<UnexcusedAbsenceOrder> builder)
    {
        builder.ToTable("unexcused_absences");

        builder.Property(x => x.EmployeeId)
            .IsRequired();

        builder.Property(x => x.SetDate)
            .IsRequired();

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
