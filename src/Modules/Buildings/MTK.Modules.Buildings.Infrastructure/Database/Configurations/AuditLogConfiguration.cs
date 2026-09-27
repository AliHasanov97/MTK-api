using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Buildings.Domain.AuditLogs;

namespace MTK.Modules.Buildings.Infrastructure.Database.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", "buildings");
        builder.HasKey(log => log.Id);
        builder.Property(log => log.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(log => log.EntityId).IsRequired();
        builder.Property(log => log.Action).IsRequired().HasMaxLength(50);
        builder.Property(log => log.OldValues).HasColumnType("jsonb");
        builder.Property(log => log.NewValues).HasColumnType("jsonb");
        builder.Property(log => log.UserId);
        builder.Property(log => log.Timestamp).IsRequired();
        builder.HasIndex(log => log.EntityType);
        builder.HasIndex(log => log.EntityId);
        builder.HasIndex(log => log.UserId);
        builder.HasIndex(log => log.Timestamp);

        // Was a plain nullable column with no generation expression, so it
        // was never populated (search always matched nothing). This makes
        // Postgres maintain it automatically.
        builder
            .HasGeneratedTsVectorColumn(
                log => log.SearchVector,
                "english",
                log => new { log.EntityType, log.Action })
            .HasIndex(log => log.SearchVector)
            .HasMethod("GIN");
    }
}
