using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Buildings.Domain.Users;

namespace MTK.Modules.Buildings.Infrastructure.Database.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.IdentityId)
            .HasMaxLength(100);

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);
        builder.Property(u => u.DeletedAt);

        builder.HasQueryFilter(u => u.DeletedAt == null);

        builder
            .HasGeneratedTsVectorColumn(
                u => u.SearchVector,
                "english",
                u => new { u.FirstName, u.LastName, u.Email })
            .HasIndex(u => u.SearchVector)
            .HasMethod("GIN");

        builder.HasIndex(u => u.Email);

        builder.HasIndex(u => u.IdentityId)
            .HasFilter("\"IdentityId\" IS NOT NULL");
    }
}
