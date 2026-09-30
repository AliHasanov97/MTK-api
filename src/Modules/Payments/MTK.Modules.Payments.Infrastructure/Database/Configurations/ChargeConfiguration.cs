using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class ChargeConfiguration : IEntityTypeConfiguration<Charge>
{
    public void Configure(EntityTypeBuilder<Charge> builder)
    {
        // Invariant bazada da qorunur: borc öz məbləğindən çox ödənilə bilməz.
        // Domendəki yoxlama (Charge.ApplyPayment) səhv kod yolunu tutur; bu isə
        // race condition və bir-başa SQL müdaxiləsi kimi hallarda son sərhəddir.
        builder.ToTable("Charges", table => table.HasCheckConstraint(
            "CK_Charges_PaidAmount_Range",
            "\"PaidAmount\" >= 0 AND \"PaidAmount\" <= \"Amount\""));

        builder.HasKey(c => c.Id);

        // PostgreSQL-in xmin sistem sütunu optimistik concurrency token-i kimi:
        // eyni borc eyni anda iki yerdən dəyişdirilsə (aylıq job + API ödənişi),
        // ikinci yazma sükutla üstünə yazmır, DbUpdateConcurrencyException verir.
        builder.Property<uint>("xmin")
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.Property(c => c.OwnerId)
            .IsRequired();

        builder.Property(c => c.PropertyType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.PropertyId)
            .IsRequired();

        builder.Property(c => c.Period)
            .IsRequired()
            .HasMaxLength(50);

        // Borcun yaşı — ödəniş/avans FIFO sırası bununla müəyyən olunur.
        builder.Property(c => c.IssuedOn)
            .IsRequired();

        builder.Property(c => c.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.PaidAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        // Snapshot fields - calculation details at time of creation
        builder.Property(c => c.AreaSquareMeters)
            .HasPrecision(10, 2);

        builder.Property(c => c.RateAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.RateType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

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
                c => new { c.Period })
            .HasIndex(c => c.SearchVector)
            .HasMethod("GIN");

        // Indexes
        builder.HasIndex(c => c.OwnerId);
        builder.HasIndex(c => c.PropertyId);
        builder.HasIndex(c => c.Period);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => new { c.OwnerId, c.IssuedOn });
        builder.HasIndex(c => new { c.OwnerId, c.PropertyId, c.Period })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");
    }
}
