using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class ChargeConfiguration : IEntityTypeConfiguration<Charge>
{
    public void Configure(EntityTypeBuilder<Charge> builder)
    {
        // Invariant bazada da qorunur: borc öz məbləğindən çox ödənilə bilməz.
        builder.ToTable("Charges", table => table.HasCheckConstraint(
            "CK_Charges_PaidAmount_Range",
            "\"PaidAmount\" >= 0 AND \"PaidAmount\" <= \"Amount\""));

        builder.HasKey(c => c.Id);

        // PostgreSQL-in xmin sistem sütunu optimistik concurrency token-i kimi.
        builder.Property<uint>("xmin")
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        // Tərəf discriminatoru — sakin və tədarükçü borclarını ayırır.
        builder.Property(c => c.PartyType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.PartyId)
            .IsRequired();

        // Sakinə xas (nullable)
        builder.Property(c => c.PropertyType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.PropertyId);

        builder.Property(c => c.AreaSquareMeters)
            .HasPrecision(10, 2);

        builder.Property(c => c.RateAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.RateType)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Tədarükçüyə xas (nullable)
        builder.Property(c => c.ContractId);
        builder.Property(c => c.ContractServiceId);

        builder.Property(c => c.DueDate);

        // Ortaq
        builder.Property(c => c.Period)
            .HasMaxLength(50);

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
                c => new { c.Period, c.Description })
            .HasIndex(c => c.SearchVector)
            .HasMethod("GIN");

        // Indexes
        builder.HasIndex(c => c.PartyType);
        builder.HasIndex(c => c.PartyId);
        builder.HasIndex(c => c.PropertyId);
        builder.HasIndex(c => c.Period);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.ContractId);
        builder.HasIndex(c => c.ContractServiceId);
        builder.HasIndex(c => c.DueDate);
        builder.HasIndex(c => new { c.PartyType, c.PartyId, c.IssuedOn });

        // Sakin borcu: eyni əmlak + dövr üçün ikinci borc yaranmasın.
        builder.HasIndex(c => new { c.PartyId, c.PropertyId, c.Period })
            .IsUnique()
            .HasFilter("\"PartyType\" = 'Owner' AND \"DeletedAt\" IS NULL");

        // Tədarükçü borcu (idempotentlik): eyni xidmət + dövr və eyni mal + qaimə.
        builder.HasIndex(c => new { c.ContractServiceId, c.Period })
            .IsUnique()
            .HasFilter("\"ContractServiceId\" IS NOT NULL AND \"DeletedAt\" IS NULL");
    }
}
