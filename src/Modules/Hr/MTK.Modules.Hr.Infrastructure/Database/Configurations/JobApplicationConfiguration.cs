
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.ToTable("job_applications");

        builder.Property(j => j.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(j => j.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.Surname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.FathersName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.Telephone)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(j => j.HomeTelephoneNumber)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(j => j.Gender)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(j => j.JobId)
            .IsRequired();

        builder.Property(j => j.StartDate)
            .IsRequired();

        builder.HasOne(j => j.Job)
            .WithMany()
            .HasForeignKey(j => j.JobId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
