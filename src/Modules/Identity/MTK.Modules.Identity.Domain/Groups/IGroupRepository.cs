namespace MTK.Modules.Identity.Domain.Groups;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Group?> GetByKeycloakGroupIdAsync(Guid keycloakGroupId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Group>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(Group group);
    void Update(Group group);
    void Remove(Group group);

    // Group membership
    Task AddUserToGroupAsync(Guid groupId, Guid userId, Guid? assignedBy, CancellationToken cancellationToken = default);
    Task RemoveUserFromGroupAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Users.User>> GetGroupMembersAsync(Guid groupId, CancellationToken cancellationToken = default);

    // Group roles
    Task AssignRolesToGroupAsync(Guid groupId, IEnumerable<Guid> roleIds, Guid? assignedBy, CancellationToken cancellationToken = default);
    Task RemoveRolesFromGroupAsync(Guid groupId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Roles.Role>> GetGroupRolesAsync(Guid groupId, CancellationToken cancellationToken = default);

    // Hierarchy
    Task<IReadOnlyList<Group>> GetSubGroupsAsync(Guid parentGroupId, CancellationToken cancellationToken = default);
}
