using MTK.Modules.Hr.Domain.EmployeeWorkHistories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class EmployeeWorkHistoryConfiguration : IEntityTypeConfiguration<EmployeeWorkHistory>
{
    public void Configure(EntityTypeBuilder<EmployeeWorkHistory> builder)
    {
        builder.ToTable("employee_work_histories");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EmployeeId)
            .IsRequired();

        builder.Property(e => e.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Position)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.StartDate)
            .IsRequired();

        builder.Property(e => e.EndDate)
            .IsRequired(false);

        builder.Property(e => e.Notes)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(e => e.CreatedById)
            .IsRequired();

        builder.HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.EmployeeId);
        builder.HasIndex(e => e.CreatedAt);
    }
}
