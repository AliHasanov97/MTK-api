using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Buildings.Domain.Appeals;

namespace MTK.Modules.Buildings.Infrastructure.Database.Configurations;

internal sealed class AppealConfiguration : IEntityTypeConfiguration<Appeal>
{
    public void Configure(EntityTypeBuilder<Appeal> builder)
    {
        builder.ToTable("appeals");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(a => a.Category)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.SubmitterId)
            .IsRequired();

        builder.Property(a => a.ApartmentId);

        builder.Property(a => a.GarageId);

        builder.Property(a => a.ResolvedAt);

        builder.Property(a => a.ClosedAt);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt);

        builder.Property(a => a.DeletedAt);

        // Soft delete filter
        builder.HasQueryFilter(a => a.DeletedAt == null);

        // Comments relationship
        builder.HasMany(a => a.Comments)
            .WithOne()
            .HasForeignKey(c => c.AppealId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.SubmitterId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.Category);
        builder.HasIndex(a => a.CreatedAt);
    }
}
