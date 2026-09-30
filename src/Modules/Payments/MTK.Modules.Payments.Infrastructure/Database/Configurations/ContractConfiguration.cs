using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Contracts;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("Contracts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Number)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.VendorId)
            .IsRequired();
        
        builder.Property(c => c.CreatedByUserId);

        builder.Property(c => c.StartDate)
            .IsRequired();

        builder.Property(c => c.EndDate)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(c => c.Note)
            .HasMaxLength(1000);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(c => c.DeletedAt == null);

        // MonthlyServices sadəcə filtrli görünüşdür (IEnumerable). EF onu naviqasiya
        // kimi tanıyıb ikinci, gözlənilməyən FK (ContractId1) yaradırdı — real
        // kolleksiya naviqasiyası yalnız Services-dir.
        builder.Ignore(c => c.MonthlyServices);

        // Search vector for full-text search
        builder
            .HasGeneratedTsVectorColumn(
                c => c.SearchVector,
                "english",
                c => new { c.Number, c.Note })
            .HasIndex(c => c.SearchVector)
            .HasMethod("GIN");

        // Indexes
        builder.HasIndex(c => c.VendorId);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.EndDate);
        builder.HasIndex(c => new {c.Number })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        // Vendor ayrı aggregate-dir → FK qoyulmur, yalnız VendorId saxlanılır.
        // Xidmətlər isə bu aggregate-in içindədir → real FK + cascade.
        builder
            .HasMany(c => c.Services)
            .WithOne()
            .HasForeignKey(s => s.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
