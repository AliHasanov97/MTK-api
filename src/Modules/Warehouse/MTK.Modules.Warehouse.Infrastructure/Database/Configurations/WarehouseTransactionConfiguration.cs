using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Infrastructure.Database.Configurations;

internal sealed class WarehouseTransactionConfiguration : IEntityTypeConfiguration<WarehouseTransaction>
{
    public void Configure(EntityTypeBuilder<WarehouseTransaction> builder)
    {
        builder.ToTable("warehouse_transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TransactionType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.NomenclatureId)
            .IsRequired();

        builder.Property(t => t.Quantity)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(t => t.TotalPrice)
            .HasPrecision(18, 2);

        builder.Property(t => t.TransactionDate)
            .IsRequired();

        builder.Property(t => t.ReferenceType)
            .HasMaxLength(100);

        builder.Property(t => t.ReferenceId);

        builder.Property(t => t.Notes)
            .HasMaxLength(500);

        builder.Property(t => t.CreatedByUserId);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.DeletedAt);

        // Relationships
        builder.HasOne(t => t.Nomenclature)
            .WithMany()
            .HasForeignKey(t => t.NomenclatureId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(t => t.NomenclatureId);
        builder.HasIndex(t => t.TransactionDate);
        builder.HasIndex(t => new { t.ReferenceType, t.ReferenceId });

        // Bir istinad (məs. alış) üçün eyni nomenklaturadan yalnız bir əməliyyat —
        // integration event-in təkrar çatdırılmasına qarşı bazada son müdafiə xətti.
        builder.HasIndex(t => new { t.ReferenceType, t.ReferenceId, t.NomenclatureId })
            .IsUnique()
            .HasFilter("\"ReferenceType\" IS NOT NULL AND \"ReferenceId\" IS NOT NULL");

        // Ignore domain events
        builder.Ignore(t => t.DomainEvents);
    }
}
