using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Groups;

public sealed class GroupRole : Entity
{
    private GroupRole(
        Guid id,
        Guid groupId,
        Guid roleId,
        Guid? assignedBy) : base(id)
    {
        GroupId = groupId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy;
    }

    // Private constructor for EF Core
    private GroupRole() : base(Guid.Empty)
    {
    }

    public Guid GroupId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public Guid? AssignedBy { get; private set; }

    public static GroupRole Create(Guid groupId, Guid roleId, Guid? assignedBy = null)
    {
        return new GroupRole(
            Guid.NewGuid(),
            groupId,
            roleId,
            assignedBy);
    }
}
