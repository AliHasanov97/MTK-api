using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Buildings.Domain.Garages;

namespace MTK.Modules.Buildings.Infrastructure.Database.Configurations;

internal sealed class GarageConfiguration : IEntityTypeConfiguration<Garage>
{
    public void Configure(EntityTypeBuilder<Garage> builder)
    {
        builder.ToTable("Garages");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.GarageNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(g => g.GarageNumber)
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        builder.Property(g => g.Type)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(g => g.OwnerId);

        builder.Property(g => g.Description)
            .HasMaxLength(500);

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        builder.Property(g => g.UpdatedAt);

        builder.Property(g => g.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(g => g.DeletedAt == null);

        // Was a plain nullable column with no generation expression, so it
        // was never populated (search always matched nothing). This makes
        // Postgres maintain it automatically.
        builder
            .HasGeneratedTsVectorColumn(
                g => g.SearchVector,
                "english",
                g => new { g.GarageNumber, g.Type, g.Description })
            .HasIndex(g => g.SearchVector)
            .HasMethod("GIN");

        // Relationships
        builder.HasOne(g => g.Owner)
            .WithMany(o => o.OwnedGarages)
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
