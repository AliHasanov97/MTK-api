using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Warehouse.Domain.WarehouseStock;

namespace MTK.Modules.Warehouse.Infrastructure.Database.Configurations;

internal sealed class WarehouseStockConfiguration : IEntityTypeConfiguration<WarehouseStock>
{
    public void Configure(EntityTypeBuilder<WarehouseStock> builder)
    {
        builder.ToTable("warehouse_stock");

        // NomenclatureId is both PK and FK
        builder.HasKey(s => s.NomenclatureId);

        builder.Property(s => s.NomenclatureId)
            .IsRequired();

        builder.Property(s => s.QuantityOnHand)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.LastTransactionDate);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        builder.Property(s => s.DeletedAt);

        // Relationships
        builder.HasOne(s => s.Nomenclature)
            .WithOne()
            .HasForeignKey<WarehouseStock>(s => s.NomenclatureId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore domain events
        builder.Ignore(s => s.DomainEvents);
    }
}
