using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.FileAttachments;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

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

        builder.HasOne(f => f.Contract)
            .WithMany()
            .HasForeignKey(f => f.ContractId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Vendor)
            .WithMany()
            .HasForeignKey(f => f.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Payment)
            .WithMany()
            .HasForeignKey(f => f.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Owner)
            .WithMany()
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Transaction)
            .WithMany()
            .HasForeignKey(f => f.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.UploadedBy)
            .WithMany()
            .HasForeignKey(f => f.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(f => f.DeletedAt == null);

        builder.HasIndex(f => f.ContractId);
        builder.HasIndex(f => f.VendorId);
        builder.HasIndex(f => f.PaymentId);
        builder.HasIndex(f => f.OwnerId);
        builder.HasIndex(f => f.TransactionId);
        builder.HasIndex(f => f.UploadedByUserId);
    }
}
