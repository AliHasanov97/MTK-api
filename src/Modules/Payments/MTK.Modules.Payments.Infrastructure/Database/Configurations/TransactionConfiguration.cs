using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Transactions;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Direction)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Category)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.TransactionDate)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(t => t.DeletedAt == null);

        // Search vector for full-text search
        builder
            .HasGeneratedTsVectorColumn(
                t => t.SearchVector,
                "english",
                t => new { t.Category, t.Description })
            .HasIndex(t => t.SearchVector)
            .HasMethod("GIN");

        // Indexes
        builder.HasIndex(t => t.Direction);
        builder.HasIndex(t => t.TransactionDate);
        builder.HasIndex(t => t.Category);
    }
}
