using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Roles;

/// <summary>
/// Represents a direct role assignment to a user (not through group membership).
/// </summary>
public sealed class UserRoleAssignment : Entity
{
    private UserRoleAssignment(
        Guid id,
        Guid userId,
        Guid roleId,
        Guid? assignedBy) : base(id)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy;
    }

    // Private constructor for EF Core
    private UserRoleAssignment() : base(Guid.Empty)
    {
    }

    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public Guid? AssignedBy { get; private set; }

    public static UserRoleAssignment Create(Guid userId, Guid roleId, Guid? assignedBy = null)
    {
        return new UserRoleAssignment(
            Guid.NewGuid(),
            userId,
            roleId,
            assignedBy);
    }
}
