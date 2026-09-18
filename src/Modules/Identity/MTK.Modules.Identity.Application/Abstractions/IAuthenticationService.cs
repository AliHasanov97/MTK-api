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
        string email,
        string firstName,
        string lastName,
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
}
