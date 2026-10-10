using MTK.Modules.Hr.Domain.NoticesOfChangeInWorkingConditions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class NoticeOfChangeInWorkingConditionsConfiguration :
    IEntityTypeConfiguration<NoticeOfChangeInWorkingConditions>
{
    public void Configure(EntityTypeBuilder<NoticeOfChangeInWorkingConditions> builder)
    {
        builder.ToTable("notice_of_change_in_working_conditions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Index)
            .IsRequired();
        builder.HasIndex(x => x.Index)
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        builder.Property(x => x.EmployeeId)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.CreatedById)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.DeletedAt);

        builder.Property(x => x.SearchVector)
            .HasColumnType("tsvector")
            .HasComputedColumnSql(
                "to_tsvector('simple', coalesce(\"Index\"::text, ''))",
                stored: true);

        builder.HasIndex(x => x.SearchVector)
            .HasMethod("GIN");

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}