using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Buildings.Domain.FileAttachments;

namespace MTK.Modules.Buildings.Infrastructure.Database.Configurations;

internal sealed class FileAttachmentConfiguration : IEntityTypeConfiguration<FileAttachment>
{
    public void Configure(EntityTypeBuilder<FileAttachment> builder)
    {
        builder.ToTable("FileAttachments");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(f => f.ObjectKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(f => f.ContentType)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(f => f.SizeBytes)
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.HasOne(f => f.Building)
            .WithMany()
            .HasForeignKey(f => f.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Apartment)
            .WithMany()
            .HasForeignKey(f => f.ApartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Garage)
            .WithMany()
            .HasForeignKey(f => f.GarageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Owner)
            .WithMany()
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(f => f.DeletedAt == null);

        builder.HasIndex(f => f.BuildingId);
        builder.HasIndex(f => f.ApartmentId);
        builder.HasIndex(f => f.GarageId);
        builder.HasIndex(f => f.OwnerId);
    }
}
