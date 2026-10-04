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

        // Tərəf — OwnerId/VendorId-dən məhz biri dolu olur (əvvəlki PartyType
        // discriminator-u əvəzinə həqiqi FK-lər; PartyType/PartyId indi Charge.cs-də
        // [NotMapped] köməkçi xassələrdir, DB sütunu deyil).
        builder.Property(c => c.OwnerId);
        builder.Property(c => c.VendorId);

        // Sakinə xas (nullable) — ApartmentId/GarageId-dən məhz biri dolu olur.
        builder.Property(c => c.ApartmentId);
        builder.Property(c => c.GarageId);

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
        builder.HasIndex(c => c.OwnerId);
        builder.HasIndex(c => c.VendorId);
        builder.HasIndex(c => c.ApartmentId);
        builder.HasIndex(c => c.GarageId);
        builder.HasIndex(c => c.Period);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.ContractId);
        builder.HasIndex(c => c.ContractServiceId);
        builder.HasIndex(c => c.DueDate);
        builder.HasIndex(c => new { c.OwnerId, c.IssuedOn });
        builder.HasIndex(c => new { c.VendorId, c.IssuedOn });

        // Sakin borcu: eyni mənzil/qaraj + dövr üçün ikinci borc yaranmasın. İki ayrı
        // filtrlənmiş unikal indeks (əvvəlki tək PartyType='Owner' indeksinin yerinə) —
        // ApartmentId/GarageId qarşılıqlı müstəsna olduğu üçün bir-birinə mane olmur.
        builder.HasIndex(c => new { c.OwnerId, c.ApartmentId, c.Period })
            .IsUnique()
            .HasFilter("\"ApartmentId\" IS NOT NULL AND \"DeletedAt\" IS NULL");
        builder.HasIndex(c => new { c.OwnerId, c.GarageId, c.Period })
            .IsUnique()
            .HasFilter("\"GarageId\" IS NOT NULL AND \"DeletedAt\" IS NULL");

        // Tədarükçü borcu (idempotentlik): eyni xidmət + dövr və eyni mal + qaimə.
        builder.HasIndex(c => new { c.ContractServiceId, c.Period })
            .IsUnique()
            .HasFilter("\"ContractServiceId\" IS NOT NULL AND \"DeletedAt\" IS NULL");

        // Naviqasiyalar — Restrict: bir shadow (Owner/Apartment/Garage) və ya Vendor
        // silinsə belə, ona istinad edən maliyyə tarixçəsi (Charge) itirilməməlidir.
        builder.HasOne(c => c.Owner)
            .WithMany()
            .HasForeignKey(c => c.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Vendor)
            .WithMany()
            .HasForeignKey(c => c.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Apartment)
            .WithMany()
            .HasForeignKey(c => c.ApartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Garage)
            .WithMany()
            .HasForeignKey(c => c.GarageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
