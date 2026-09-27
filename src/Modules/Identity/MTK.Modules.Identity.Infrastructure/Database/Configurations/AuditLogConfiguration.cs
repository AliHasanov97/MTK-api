using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Identity.Domain.AuditLogs;

namespace MTK.Modules.Identity.Infrastructure.Database.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", "identity");

        builder.HasKey(al => al.Id);

        builder.Property(al => al.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(al => al.EntityId)
            .IsRequired();

        builder.Property(al => al.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(al => al.OldValues)
            .HasColumnType("jsonb");

        builder.Property(al => al.NewValues)
            .HasColumnType("jsonb");

        builder.Property(al => al.UserId);

        builder.Property(al => al.Timestamp)
            .IsRequired();

        builder.Property(al => al.CreatedAt)
            .IsRequired();

        builder.Property(al => al.UpdatedAt);

        builder.Property(al => al.DeletedAt);

        // Global soft delete filter
        builder.HasQueryFilter(al => al.DeletedAt == null);

        // SearchVector for full-text search
        builder
            .HasGeneratedTsVectorColumn(
                al => al.SearchVector,
                "english",
                al => new { al.EntityType, al.Action })
            .HasIndex(al => al.SearchVector)
            .HasMethod("GIN");

        // Indexes for efficient querying
        builder.HasIndex(al => al.EntityType);
        builder.HasIndex(al => al.EntityId);
        builder.HasIndex(al => al.UserId);
        builder.HasIndex(al => al.Timestamp);
    }
}
