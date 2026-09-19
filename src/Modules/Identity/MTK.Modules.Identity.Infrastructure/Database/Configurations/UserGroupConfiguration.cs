using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Identity.Domain.Groups;

namespace MTK.Modules.Identity.Infrastructure.Database.Configurations;

public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        builder.ToTable("UserGroups", "identity");

        builder.HasKey(ug => ug.Id);

        builder.Property(ug => ug.UserId)
            .IsRequired();

        builder.Property(ug => ug.GroupId)
            .IsRequired();

        builder.Property(ug => ug.AssignedAt)
            .IsRequired();

        builder.Property(ug => ug.AssignedBy);

        // Composite unique index to prevent duplicate assignments
        builder.HasIndex(ug => new { ug.UserId, ug.GroupId })
            .IsUnique();

        // Foreign keys
        builder.HasOne<Domain.Users.User>()
            .WithMany()
            .HasForeignKey(ug => ug.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Group>()
            .WithMany()
            .HasForeignKey(ug => ug.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
