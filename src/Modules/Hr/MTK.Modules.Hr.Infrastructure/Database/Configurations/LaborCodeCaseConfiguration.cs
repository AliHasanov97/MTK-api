using MTK.Modules.Hr.Domain.LaborCodeCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class LaborCodeCaseConfiguration : IEntityTypeConfiguration<LaborCodeCase>
{
    public void Configure(EntityTypeBuilder<LaborCodeCase> builder)
    {
        builder.ToTable("labor_code_cases");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(l => l.ParentId)
            .IsRequired(false);

        builder.Property(l => l.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(l => l.Parent)
            .WithMany(l => l.Children)
            .HasForeignKey(l => l.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Metadata
            .FindNavigation(nameof(LaborCodeCase.Children))!
            .SetField("_children");

        builder.Property(l => l.SearchVector)
            .HasComputedColumnSql(
                "to_tsvector('simple', coalesce(\"Code\",'') || ' ' || coalesce(\"Name\",''))",
                stored: true);

        builder.HasIndex(l => l.SearchVector).HasMethod("gin");
        builder.HasIndex(l => l.Code);
        builder.HasIndex(l => l.ParentId);
        builder.HasIndex(l => l.IsActive);
    }
}
