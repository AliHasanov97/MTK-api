namespace MTK.Modules.Identity.Application.Abstractions;

public interface IAuthenticationService
{
    // User Management
    Task<string> RegisterAsync(
        string email,
        string firstName,
        string lastName,
        string password,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string userIdentityId, CancellationToken cancellationToken = default);

    Task UpdateUserAsync(
        string userId,
        string firstName,
        string lastName,
        string email,
        string? phoneNumber,
        CancellationToken cancellationToken = default);

    Task UpdateUserPasswordAsync(
        string userId,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<bool> VerifyPasswordAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default);

    // Role Management
    Task CreateRealmRoleAsync(
        string name,
        string description,
        CancellationToken cancellationToken = default);

    Task DeleteRealmRoleAsync(
        string roleName,
        CancellationToken cancellationToken = default);

    Task UpdateRealmRoleAsync(
        string roleName,
        string newName,
        string description,
        CancellationToken cancellationToken = default);

    Task<List<string>> GetRealmRoleNamesAsync(CancellationToken cancellationToken = default);

    Task AssignRealmRoleToUserAsync(
        string userId,
        string roleName,
        CancellationToken cancellationToken = default);

    Task AssignRealmRolesToUserAsync(
        string userId,
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default);

    Task RemoveRealmRolesFromUserAsync(
        string userId,
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default);

    Task<List<string>> GetUserDirectRoleNamesAsync(
        string userId,
        CancellationToken cancellationToken = default);

    // Group Management
    Task<List<GroupDto>> GetAllGroupsAsync(CancellationToken cancellationToken = default);

    Task<List<GroupDto>> SearchGroupsAsync(
        string? searchTerm,
        CancellationToken cancellationToken = default);

    Task<GroupDto?> GetGroupByIdAsync(
        Guid groupId,
        CancellationToken cancellationToken = default);

    Task<List<UserDto>> GetGroupMembersAsync(
        Guid groupId,
        CancellationToken cancellationToken = default);

    Task<List<GroupDto>> GetUserGroupsAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateGroupAsync(
        string name,
        string? description,
        CancellationToken cancellationToken = default);

    Task UpdateGroupAsync(
        Guid keycloakGroupId,
        string name,
        string? description,
        CancellationToken cancellationToken = default);

    Task DeleteGroupAsync(
        Guid keycloakGroupId,
        CancellationToken cancellationToken = default);

    Task AddUserToGroupAsync(
        string identityId,
        Guid keycloakGroupId,
        CancellationToken cancellationToken = default);

    Task RemoveUserFromGroupAsync(
        string identityId,
        Guid keycloakGroupId,
        CancellationToken cancellationToken = default);

    // Group-Role Management
    Task AssignRolesToGroupAsync(
        Guid keycloakGroupId,
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default);

    Task RemoveRolesFromGroupAsync(
        Guid keycloakGroupId,
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> GetGroupRoleNamesAsync(
        Guid keycloakGroupId,
        CancellationToken cancellationToken = default);
}
