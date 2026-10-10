using MTK.Modules.Hr.Domain.CalendarDays;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class CalendarDayConfiguration : IEntityTypeConfiguration<CalendarDay>
{
    public void Configure(EntityTypeBuilder<CalendarDay> builder)
    {
        builder.ToTable("calendar_days");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Date)
            .IsRequired();

        builder.Property(c => c.Year)
            .IsRequired();

        builder.Property(c => c.DayType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.Reason)
            .HasMaxLength(500);

        builder.Property(c => c.ApplicableWorkingDays)
            .IsRequired(false)
            .HasConversion<int?>();

        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt);
        builder.Property(c => c.DeletedAt);

        // Full-text search
        builder.Property(c => c.SearchVector)
            .HasComputedColumnSql(
                "to_tsvector('simple', coalesce(\"Name\",''))",
                stored: true);

        builder.HasIndex(c => c.SearchVector)
            .HasMethod("gin");

        // Indexes
        builder.HasIndex(c => c.Date);
        builder.HasIndex(c => c.Year);
        builder.HasIndex(c => new { c.Year, c.Date });
        builder.HasIndex(c => c.DayType);
        builder.HasIndex(c => c.DeletedAt);
        builder.HasIndex(c => new { c.Date, c.ApplicableWorkingDays });
    }
}
