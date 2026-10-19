using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Communications.Domain.Articles;

namespace MTK.Modules.Communications.Infrastructure.Database.Configurations;

internal sealed class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("articles");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Summary)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.Content)
            .IsRequired();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(ArticleStatus.Draft);

        builder.HasIndex(a => a.Status);

        builder.Property(a => a.PublishedAt);

        builder.Property(a => a.CreatedById)
            .IsRequired();

        builder.HasOne(a => a.CreatedBy)
            .WithMany()
            .HasForeignKey(a => a.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.FileAttachments)
            .WithOne(f => f.Article)
            .HasForeignKey(f => f.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(a => a.ViewCount)
            .HasDefaultValue(0);

        builder.HasIndex(a => a.CreatedAt);
        builder.HasIndex(a => a.PublishedAt);

        // Full-text search configuration
        builder.Property(a => a.SearchVector)
            .HasColumnType("tsvector")
            .HasComputedColumnSql(
                "to_tsvector('simple', coalesce(\"Title\", '') || ' ' || coalesce(\"Summary\", '') || ' ' || coalesce(\"Content\", ''))",
                stored: true);

        builder.HasIndex(a => a.SearchVector)
            .HasMethod("gin");
    }
}
