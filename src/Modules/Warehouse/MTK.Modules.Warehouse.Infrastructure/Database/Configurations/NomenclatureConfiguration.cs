using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Infrastructure.Database.Configurations;

internal sealed class NomenclatureConfiguration : IEntityTypeConfiguration<Nomenclature>
{
    public void Configure(EntityTypeBuilder<Nomenclature> builder)
    {
        builder.ToTable("nomenclatures");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(n => n.Code)
            .IsUnique();

        builder.Property(n => n.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Description)
            .HasMaxLength(1000);

        builder.Property(n => n.Category)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(n => n.Unit)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(n => n.MinStockLevel)
            .HasPrecision(18, 2);

        builder.Property(n => n.IsActive)
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.UpdatedAt);

        builder.Property(n => n.DeletedAt);

        // Soft delete query filter
        builder.HasQueryFilter(n => n.DeletedAt == null);

        // Search vector for full-text search (Nomenclature derives from SearchableEntity)
        builder
            .HasGeneratedTsVectorColumn(
                n => n.SearchVector,
                "english",
                n => new { n.Code, n.Name, n.Description })
            .HasIndex(n => n.SearchVector)
            .HasMethod("GIN");

        // Ignore domain events
        builder.Ignore(n => n.DomainEvents);
    }
}
