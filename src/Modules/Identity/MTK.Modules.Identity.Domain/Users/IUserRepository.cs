namespace MTK.Modules.Identity.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdentityIdAsync(string identityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(User user);
    void Update(User user);
    void Remove(User user);

    // User-Role assignments
    Task AssignRolesToUserAsync(Guid userId, IEnumerable<Guid> roleIds, Guid? assignedBy, CancellationToken cancellationToken = default);
    Task RemoveRolesFromUserAsync(Guid userId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Roles.Role>> GetUserDirectRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Roles.Role>> GetUserEffectiveRolesAsync(Guid userId, CancellationToken cancellationToken = default); // Direct + via groups

    // User-Group assignments
    Task<IReadOnlyList<Groups.Group>> GetUserGroupsAsync(Guid userId, CancellationToken cancellationToken = default);
}
