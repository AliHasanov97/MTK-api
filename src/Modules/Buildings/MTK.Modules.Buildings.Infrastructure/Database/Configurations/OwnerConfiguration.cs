using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Buildings.Domain.Owners;

namespace MTK.Modules.Buildings.Infrastructure.Database.Configurations;

internal sealed class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        builder.ToTable("Owners");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.UserId)
            .IsRequired(false); // Nullable - passive owners don't have user accounts

        builder.HasIndex(o => o.UserId)
            .IsUnique()
            .HasFilter("\"UserId\" IS NOT NULL AND \"DeletedAt\" IS NULL"); // Unique only for non-null UserIds

        builder.Property(o => o.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(o => o.PhoneNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt);

        builder.Property(o => o.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(o => o.DeletedAt == null);

        // SearchVector was previously just a plain nullable column with no
        // generation expression, so it was never populated (search always
        // matched nothing). This makes Postgres maintain it automatically.
        builder
            .HasGeneratedTsVectorColumn(
                o => o.SearchVector,
                "english",
                o => new { o.FirstName, o.LastName, o.Email, o.PhoneNumber })
            .HasIndex(o => o.SearchVector)
            .HasMethod("GIN");

        // Relationships
        builder.HasMany(o => o.OwnedApartments)
            .WithOne(a => a.CurrentOwner)
            .HasForeignKey(a => a.CurrentOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.OwnedGarages)
            .WithOne(g => g.Owner)
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
