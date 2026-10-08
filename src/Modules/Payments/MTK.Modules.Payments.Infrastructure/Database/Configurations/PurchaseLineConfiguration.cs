using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Purchases;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class PurchaseLineConfiguration : IEntityTypeConfiguration<PurchaseLine>
{
    public void Configure(EntityTypeBuilder<PurchaseLine> builder)
    {
        builder.ToTable("PurchaseLines");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.PurchaseId)
            .IsRequired();

        builder.Property(l => l.NomenclatureId)
            .IsRequired();

        builder.Property(l => l.Quantity)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(l => l.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(l => l.CreatedAt).IsRequired();
        builder.Property(l => l.UpdatedAt);
        builder.Property(l => l.DeletedAt);

        // LineTotal hesablanan property-dir — sütun kimi saxlanılmır.
        builder.Ignore(l => l.LineTotal);

        // Kolleksiyadan çıxarılan sətir bazadan fiziki silinmir (ümumi soft delete
        // qaydası), ona görə filter olmadan yüklənən naviqasiyada yenidən görünərdi.
        builder.HasQueryFilter(l => l.DeletedAt == null);

        builder.HasIndex(l => l.PurchaseId);
        builder.HasIndex(l => l.NomenclatureId);
    }
}
