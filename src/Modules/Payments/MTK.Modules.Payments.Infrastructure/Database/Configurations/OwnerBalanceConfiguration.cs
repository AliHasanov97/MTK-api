using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.OwnerBalances;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class OwnerBalanceConfiguration : IEntityTypeConfiguration<OwnerBalance>
{
    public void Configure(EntityTypeBuilder<OwnerBalance> builder)
    {
        builder.ToTable("OwnerBalances");

        builder.HasKey(ob => ob.Id);

        builder.Property(ob => ob.OwnerId)
            .IsRequired();

        builder.Property(ob => ob.TotalDebt)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(ob => ob.TotalPaid)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(ob => ob.CreatedAt)
            .IsRequired();

        builder.Property(ob => ob.UpdatedAt);

        builder.Property(ob => ob.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(ob => ob.DeletedAt == null);

        // Search vector - OwnerBalance has no text fields, so we use a minimal configuration
        // This creates an empty search vector to maintain consistency across entities
        builder.Property(ob => ob.SearchVector)
            .HasColumnType("tsvector")
            .HasComputedColumnSql("''::tsvector", stored: true);

        // Indexes
        builder.HasIndex(ob => ob.OwnerId)
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");
    }
}
