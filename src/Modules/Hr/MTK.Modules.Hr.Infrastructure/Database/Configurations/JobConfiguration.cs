using MTK.Modules.Hr.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("jobs");
        builder.HasKey(j => j.Id);
        builder.Property(j => j.Id).ValueGeneratedOnAdd();

        builder.Property(j => j.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(j => j.Name);

        builder.Property(j => j.CreatedAt).IsRequired();
        builder.Property(j => j.UpdatedAt);

        builder.Property(e => e.SearchVector)
            .HasComputedColumnSql(
                "to_tsvector('simple', coalesce(\"Name\",''))",
                stored: true);

        builder.HasIndex(e => e.SearchVector)
            .HasMethod("gin");
    }
}
