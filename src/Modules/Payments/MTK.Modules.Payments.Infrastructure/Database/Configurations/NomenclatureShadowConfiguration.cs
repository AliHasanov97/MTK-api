using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Nomenclatures;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class NomenclatureShadowConfiguration : IEntityTypeConfiguration<NomenclatureShadow>
{
    public void Configure(EntityTypeBuilder<NomenclatureShadow> builder)
    {
        builder.ToTable("NomenclatureShadows");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(n => n.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Description)
            .HasMaxLength(1000);

        builder.Property(n => n.Category)
            .IsRequired();

        builder.Property(n => n.Unit)
            .IsRequired();

        builder.Property(n => n.MinStockLevel)
            .HasPrecision(18, 2);

        builder.Property(n => n.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(n => n.SyncedAtUtc)
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.UpdatedAt);
        builder.Property(n => n.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(n => n.DeletedAt == null);

        // Search vector for full-text search
        builder
            .HasGeneratedTsVectorColumn(
                n => n.SearchVector,
                "english",
                n => new { n.Code, n.Name, n.Description })
            .HasIndex(n => n.SearchVector)
            .HasMethod("GIN");

        // Warehouse-dakı kod unikaldır — güzgüdə də (soft-delete olunmuş sətirlər istisna)
        builder.HasIndex(n => n.Code)
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        builder.HasIndex(n => n.IsActive);
    }
}
