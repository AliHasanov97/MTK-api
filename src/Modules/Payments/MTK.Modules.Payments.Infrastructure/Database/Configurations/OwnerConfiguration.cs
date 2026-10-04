using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Payments.Domain.Owners;

namespace MTK.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        builder.ToTable("owners");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.FullName)
            .IsRequired()
            .HasMaxLength(200);
    }
}
