using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Hr.Domain.VacationBalances;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class VacationBalanceConfiguration : IEntityTypeConfiguration<VacationBalance>
{
    public void Configure(EntityTypeBuilder<VacationBalance> builder)
    {
        builder.ToTable("vacation_balances");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.EmployeeId).IsRequired();
        builder.Property(b => b.WorkYearStart).IsRequired();
        builder.Property(b => b.WorkYearEnd).IsRequired();
        builder.Property(b => b.BaseEntitledDays).IsRequired();
        builder.Property(b => b.ExperienceBonusDays).IsRequired();
        builder.Property(b => b.KarabakhWorkerDays).IsRequired();
        builder.Property(b => b.ChildrenUnder14Days).IsRequired();
        builder.Property(b => b.DisabledChildDays).IsRequired();
        builder.Property(b => b.UsedDays).IsRequired().HasDefaultValue(0);
        builder.Property(b => b.CompensatedDays).IsRequired().HasDefaultValue(0);

        builder.Property(b => b.CreatedAt).IsRequired();
        builder.Property(b => b.UpdatedAt);
        builder.Property(b => b.DeletedAt);

        builder.HasQueryFilter(b => b.DeletedAt == null);

        // Computed-only — EF-ə naviqasiya kimi görünməsin.
        builder.Ignore(b => b.AdditionalDays);
        builder.Ignore(b => b.TotalEntitledDays);
        builder.Ignore(b => b.RemainingDays);

        builder.HasOne(b => b.Employee)
            .WithMany()
            .HasForeignKey(b => b.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Eyni işçi üçün eyni iş ilinin iki balans sətri ola bilməz.
        builder.HasIndex(b => new { b.EmployeeId, b.WorkYearStart })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");
    }
}
