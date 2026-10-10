using MTK.Modules.Hr.Domain.EmployeeEducationHistories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class EmployeeEducationHistoryConfiguration : IEntityTypeConfiguration<EmployeeEducationHistory>
{
    public void Configure(EntityTypeBuilder<EmployeeEducationHistory> builder)
    {
        builder.ToTable("employee_education_histories");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EmployeeId).IsRequired();

        builder.Property(e => e.EducationalInstitutionId).IsRequired();

        builder.Property(e => e.EducationLevel)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.Faculty)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Specialty)
            .IsRequired(false)
            .HasMaxLength(200);

        builder.Property(e => e.StartDate).IsRequired();
        builder.Property(e => e.EndDate).IsRequired(false);

        builder.Property(e => e.DiplomaNumber)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(e => e.RegisterNumber)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(e => e.CreatedById).IsRequired();

        builder.HasOne(e => e.EducationalInstitution)
            .WithMany()
            .HasForeignKey(e => e.EducationalInstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.EmployeeId);
        builder.HasIndex(e => e.EducationalInstitutionId);
        builder.HasIndex(e => e.EndDate);
    }
}
