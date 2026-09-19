using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTK.Modules.Identity.Domain.Groups;

namespace MTK.Modules.Identity.Infrastructure.Database.Configurations;

public class GroupRoleConfiguration : IEntityTypeConfiguration<GroupRole>
{
    public void Configure(EntityTypeBuilder<GroupRole> builder)
    {
        builder.ToTable("GroupRoles", "identity");

        builder.HasKey(gr => gr.Id);

        builder.Property(gr => gr.GroupId)
            .IsRequired();

        builder.Property(gr => gr.RoleId)
            .IsRequired();

        builder.Property(gr => gr.AssignedAt)
            .IsRequired();

        builder.Property(gr => gr.AssignedBy);

        // Composite unique index to prevent duplicate assignments
        builder.HasIndex(gr => new { gr.GroupId, gr.RoleId })
            .IsUnique();

        // Foreign keys
        builder.HasOne<Group>()
            .WithMany()
            .HasForeignKey(gr => gr.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Domain.Roles.Role>()
            .WithMany()
            .HasForeignKey(gr => gr.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
