using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Contracts;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class ContractGoodsItemConfiguration : IEntityTypeConfiguration<ContractGoodsItem>
{
    public void Configure(EntityTypeBuilder<ContractGoodsItem> builder)
    {
        builder.ToTable("ContractGoodsItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ContractId)
            .IsRequired();

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(i => i.Description)
            .HasMaxLength(1000);

        builder.Property(i => i.Unit)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.AgreedQuantity)
            .HasPrecision(18, 2);

        builder.Property(i => i.PaymentTermDays);

        builder.Property(i => i.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.UpdatedAt);

        builder.Property(i => i.DeletedAt);

        // Xidmətlərdəki kimi: kolleksiyadan çıxarılan sətir fiziki silinmir.
        builder.HasQueryFilter(i => i.DeletedAt == null);

        // Indexes
        builder.HasIndex(i => i.ContractId);
        builder.HasIndex(i => i.IsActive);
    }
}
