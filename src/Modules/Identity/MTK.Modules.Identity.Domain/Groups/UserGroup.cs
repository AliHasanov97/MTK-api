using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Groups;

public sealed class UserGroup : Entity
{
    private UserGroup(
        Guid id,
        Guid userId,
        Guid groupId,
        Guid? assignedBy) : base(id)
    {
        UserId = userId;
        GroupId = groupId;
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy;
    }

    // Private constructor for EF Core
    private UserGroup() : base(Guid.Empty)
    {
    }

    public Guid UserId { get; private set; }
    public Guid GroupId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public Guid? AssignedBy { get; private set; }

    public static UserGroup Create(Guid userId, Guid groupId, Guid? assignedBy = null)
    {
        return new UserGroup(
            Guid.NewGuid(),
            userId,
            groupId,
            assignedBy);
    }
}
