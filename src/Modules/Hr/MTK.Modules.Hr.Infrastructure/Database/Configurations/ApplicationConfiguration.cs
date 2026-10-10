using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApplicationEntity = MTK.Modules.Hr.Domain.Applications.Application;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class ApplicationConfiguration : IEntityTypeConfiguration<ApplicationEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationEntity> builder)
    {
        builder.ToTable("applications");
        builder.UseTptMappingStrategy();

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ApplicationNumber)
            .IsRequired()
            .HasDefaultValueSql("nextval('hr.application_number_seq')");

        builder.HasIndex(a => a.ApplicationNumber)
            .IsUnique();

        builder.Property(a => a.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasDefaultValue(Domain.Applications.ApplicationStatus.PendingApproval);

        builder.HasIndex(a => a.Status);

        builder.Property(a => a.CreatedById)
            .IsRequired();

        builder.HasOne(a => a.CreatedBy)
            .WithMany()
            .HasForeignKey(a => a.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.FileAttachments)
            .WithOne()
            .HasForeignKey("ApplicationId")
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore computed navigation properties
        builder.Ignore(a => a.RelatedEmployee);
        builder.Ignore(a => a.RelatedJobApplicant);
        builder.Ignore(a => a.RelatedOrder);

        builder.HasIndex(a => a.CreatedAt);

        builder.Property(a => a.SearchVector)
            .HasColumnType("tsvector")
            .HasComputedColumnSql(
                "to_tsvector('simple', coalesce(\"ApplicationNumber\"::text, ''))",
                stored: true);

        builder.HasIndex(a => a.SearchVector)
            .HasMethod("gin");
    }
}
