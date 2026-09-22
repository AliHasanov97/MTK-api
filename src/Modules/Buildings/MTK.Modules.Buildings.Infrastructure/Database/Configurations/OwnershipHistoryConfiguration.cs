using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Buildings.Domain.OwnershipHistories;

namespace MTK.Modules.Buildings.Infrastructure.Database.Configurations;

internal sealed class OwnershipHistoryConfiguration : IEntityTypeConfiguration<OwnershipHistory>
{
    public void Configure(EntityTypeBuilder<OwnershipHistory> builder)
    {
        builder.ToTable("OwnershipHistories");

        builder.HasKey(oh => oh.Id);

        builder.Property(oh => oh.ApartmentId)
            .IsRequired();

        builder.HasIndex(oh => new { oh.ApartmentId, oh.TransferDate });

        builder.Property(oh => oh.PreviousOwnerId);

        builder.Property(oh => oh.PreviousOwnerName)
            .HasMaxLength(200);

        builder.Property(oh => oh.NewOwnerId)
            .IsRequired();

        builder.Property(oh => oh.NewOwnerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(oh => oh.TransferDate)
            .IsRequired();

        builder.Property(oh => oh.SalePrice)
            .HasPrecision(18, 2);

        builder.Property(oh => oh.Notes)
            .HasMaxLength(1000);

        builder.Property(oh => oh.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(oh => oh.Apartment)
            .WithMany()
            .HasForeignKey(oh => oh.ApartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(oh => oh.PreviousOwner)
            .WithMany()
            .HasForeignKey(oh => oh.PreviousOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(oh => oh.NewOwner)
            .WithMany()
            .HasForeignKey(oh => oh.NewOwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
