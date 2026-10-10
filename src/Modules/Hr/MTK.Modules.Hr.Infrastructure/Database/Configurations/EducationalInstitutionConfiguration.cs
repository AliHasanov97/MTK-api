using MTK.Modules.Hr.Domain.EducationalInstitutions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class EducationalInstitutionConfiguration : IEntityTypeConfiguration<EducationalInstitution>
{
    public void Configure(EntityTypeBuilder<EducationalInstitution> builder)
    {
        builder.ToTable("educational_institutions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.HasIndex(e => e.Name);

        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.SearchVector)
            .HasComputedColumnSql(
                "to_tsvector('simple', coalesce(\"Name\",''))",
                stored: true);

        builder.HasIndex(e => e.SearchVector)
            .HasMethod("gin");
    }
}
