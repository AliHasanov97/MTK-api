using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Buildings.Domain.Appeals;

namespace MTK.Modules.Buildings.Infrastructure.Database.Configurations;

internal sealed class AppealCommentConfiguration : IEntityTypeConfiguration<AppealComment>
{
    public void Configure(EntityTypeBuilder<AppealComment> builder)
    {
        builder.ToTable("appeal_comments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.AppealId)
            .IsRequired();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.Text)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.DeletedAt);

        // Soft delete filter
        builder.HasQueryFilter(c => c.DeletedAt == null);

        // Indexes
        builder.HasIndex(c => c.AppealId);
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.CreatedAt);
    }
}
