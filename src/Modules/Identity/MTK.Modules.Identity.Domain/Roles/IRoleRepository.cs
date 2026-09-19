namespace MTK.Modules.Identity.Domain.Roles;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetByTypeAsync(RoleType roleType, CancellationToken cancellationToken = default);
    void Add(Role role);
    void Update(Role role);
    void Remove(Role role);

    // Role assignments
    Task<IReadOnlyList<Groups.Group>> GetGroupsWithRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Users.User>> GetUsersWithDirectRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
}
