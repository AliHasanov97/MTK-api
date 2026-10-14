using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Customers;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(c => c.CustomerType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Voen)
            .HasMaxLength(50);

        builder.Property(c => c.Director)
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .HasMaxLength(200);

        builder.Property(c => c.Phone)
            .HasMaxLength(50);

        builder.Property(c => c.Address)
            .HasMaxLength(500);

        builder.Property(c => c.Note)
            .HasMaxLength(1000);

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(c => c.DeletedAt == null);

        // Search vector for full-text search
        builder
            .HasGeneratedTsVectorColumn(
                c => c.SearchVector,
                "english",
                c => new { c.Name, c.Voen, c.Director, c.Email })
            .HasIndex(c => c.SearchVector)
            .HasMethod("GIN");

        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.IsActive);
        builder.HasIndex(c => c.Voen)
            .IsUnique()
            .HasFilter("\"Voen\" IS NOT NULL AND \"DeletedAt\" IS NULL");
    }
}
