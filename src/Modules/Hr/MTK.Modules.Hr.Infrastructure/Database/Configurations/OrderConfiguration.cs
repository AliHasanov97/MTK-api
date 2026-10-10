using MTK.Modules.Hr.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MTK.Modules.Hr.Infrastructure.Database.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.UseTptMappingStrategy();

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasDefaultValueSql("nextval('hr.order_number_seq')");

        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.Property(o => o.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(o => o.CreatedById)
            .IsRequired();

        builder.HasOne(o => o.CreatedBy)
            .WithMany()
            .HasForeignKey(o => o.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.FileAttachments)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore computed navigation properties
        builder.Ignore(o => o.RelatedEmployee);
        builder.Ignore(o => o.RelatedJobApplicant);

        builder.HasIndex(o => o.CreatedAt);

        builder.Property(o => o.SearchVector)
            .HasColumnType("tsvector")
            .HasComputedColumnSql(
                "to_tsvector('simple', coalesce(\"OrderNumber\"::text, ''))",
                stored: true);

        builder.HasIndex(o => o.SearchVector)
            .HasMethod("gin");
    }
}
