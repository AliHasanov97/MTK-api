using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Contracts;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class ContractServiceConfiguration : IEntityTypeConfiguration<ContractService>
{
    public void Configure(EntityTypeBuilder<ContractService> builder)
    {
        builder.ToTable("ContractServices");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ContractId)
            .IsRequired();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        builder.Property(s => s.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.BillingPeriod)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        builder.Property(s => s.DeletedAt);

        // Kolleksiyadan çıxarılan xidmət bazadan fiziki silinmir (ümumi soft delete
        // qaydası), ona görə filter olmadan yüklənən naviqasiyada yenidən görünərdi.
        builder.HasQueryFilter(s => s.DeletedAt == null);

        // Indexes
        builder.HasIndex(s => s.ContractId);
        builder.HasIndex(s => s.IsActive);
    }
}
