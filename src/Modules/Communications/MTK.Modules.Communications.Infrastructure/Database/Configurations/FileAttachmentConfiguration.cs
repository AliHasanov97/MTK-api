using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Communications.Domain.FileAttachments;

namespace MTK.Modules.Communications.Infrastructure.Database.Configurations;

internal sealed class FileAttachmentConfiguration : IEntityTypeConfiguration<FileAttachment>
{
    public void Configure(EntityTypeBuilder<FileAttachment> builder)
    {
        builder.ToTable("file_attachments");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(f => f.MimeType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.IsPublic)
            .IsRequired();

        builder.Property(f => f.ArticleId)
            .IsRequired();

        builder.HasOne(f => f.Article)
            .WithMany(a => a.FileAttachments)
            .HasForeignKey(f => f.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => f.ArticleId);

        // Full-text search configuration
        builder.Property(f => f.SearchVector)
            .HasColumnType("tsvector")
            .HasComputedColumnSql(
                "to_tsvector('simple', coalesce(\"FileName\", ''))",
                stored: true);

        builder.HasIndex(f => f.SearchVector)
            .HasMethod("gin");

        // Ignore domain events
        builder.Ignore(f => f.DomainEvents);
    }
}
