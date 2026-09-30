using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Vendors;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.ToTable("Vendors");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(v => v.VendorType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Voen)
            .HasMaxLength(50);

        builder.Property(v => v.Director)
            .HasMaxLength(200);

        builder.Property(v => v.Email)
            .HasMaxLength(200);

        builder.Property(v => v.Phone)
            .HasMaxLength(50);

        builder.Property(v => v.Address)
            .HasMaxLength(500);

        builder.Property(v => v.Note)
            .HasMaxLength(1000);

        builder.Property(v => v.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.UpdatedAt);

        builder.Property(v => v.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(v => v.DeletedAt == null);

        // Search vector for full-text search
        builder
            .HasGeneratedTsVectorColumn(
                v => v.SearchVector,
                "english",
                v => new { v.Name, v.Voen, v.Director, v.Email })
            .HasIndex(v => v.SearchVector)
            .HasMethod("GIN");

        // Indexes
        builder.HasIndex(v => v.Name);
        builder.HasIndex(v => v.IsActive); builder.HasIndex(v => v.Voen)
            .IsUnique()
            .HasFilter("\"Voen\" IS NOT NULL AND \"DeletedAt\" IS NULL");
        
    }
}
