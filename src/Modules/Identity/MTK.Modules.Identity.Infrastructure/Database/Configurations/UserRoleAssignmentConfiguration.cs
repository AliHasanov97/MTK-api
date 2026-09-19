using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Identity.Domain.Roles;

namespace MTK.Modules.Identity.Infrastructure.Database.Configurations;

public class UserRoleAssignmentConfiguration : IEntityTypeConfiguration<UserRoleAssignment>
{
    public void Configure(EntityTypeBuilder<UserRoleAssignment> builder)
    {
        builder.ToTable("UserRoleAssignments", "identity");

        builder.HasKey(ura => ura.Id);

        builder.Property(ura => ura.UserId)
            .IsRequired();

        builder.Property(ura => ura.RoleId)
            .IsRequired();

        builder.Property(ura => ura.AssignedAt)
            .IsRequired();

        builder.Property(ura => ura.AssignedBy);

        // Composite unique index to prevent duplicate assignments
        builder.HasIndex(ura => new { ura.UserId, ura.RoleId })
            .IsUnique();

        // Foreign keys
        builder.HasOne<Domain.Users.User>()
            .WithMany()
            .HasForeignKey(ura => ura.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(ura => ura.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
