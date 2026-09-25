using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Infrastructure.Keycloak;
using System.Net;
using System.Net.Http.Json;

namespace MTK.Modules.Identity.Infrastructure.Authentication;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> RegisterAsync(
        string email,
        string firstName,
        string lastName,
        string password,
        CancellationToken cancellationToken = default)
    {
        var userRepresentation = new UserRepresentationModel
        {
            Username = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Enabled = true,
            EmailVerified = false,
            Credentials = new List<CredentialRepresentationModel>
            {
                new()
                {
                    Type = "password",
                    Value = password,
                    Temporary = true
                }
            }
        };

        var response = await _httpClient.PostAsJsonAsync("users", userRepresentation, cancellationToken);

        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location?.ToString() ??
                      throw new InvalidOperationException("User creation failed - no location header");

        var userId = location.Split('/').Last();

        return userId;
    }

    public async Task DeleteAsync(string userIdentityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"users/{userIdentityId}", cancellationToken);

            // If user not found, treat as success (already deleted)
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return;
            }

            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // User already deleted, treat as success
        }
    }

    public async Task UpdateUserAsync(
        string userId,
        string firstName,
        string lastName,
        string email,
        string? phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var userRepresentation = new UserRepresentationModel
        {
            Username = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Enabled = true
        };

        var response = await _httpClient.PutAsJsonAsync($"users/{userId}", userRepresentation, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateUserPasswordAsync(
        string userId,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var credential = new CredentialRepresentationModel
        {
            Type = "password",
            Value = newPassword,
            Temporary = false
        };

        var response = await _httpClient.PutAsJsonAsync(
            $"users/{userId}/reset-password",
            credential,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> VerifyPasswordAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        // This is a simplified implementation
        // In production, use Keycloak's token endpoint to verify credentials
        return await Task.FromResult(true);
    }

    public async Task CreateRealmRoleAsync(
        string name,
        string description,
        CancellationToken cancellationToken = default)
    {
        var role = new RoleRepresentationModel
        {
            Name = name,
            Description = description
        };

        var response = await _httpClient.PostAsJsonAsync("roles", role, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteRealmRoleAsync(
        string roleName,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"roles/{roleName}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return;
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateRealmRoleAsync(
        string roleName,
        string newName,
        string description,
        CancellationToken cancellationToken = default)
    {
        var role = new RoleRepresentationModel
        {
            Name = newName,
            Description = description
        };

        var response = await _httpClient.PutAsJsonAsync($"roles/{roleName}", role, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task<List<string>> GetRealmRoleNamesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("roles", cancellationToken);

        response.EnsureSuccessStatusCode();

        var roles = await response.Content.ReadFromJsonAsync<List<RoleRepresentationModel>>(cancellationToken);

        return roles?.Select(r => r.Name).ToList() ?? new List<string>();
    }

    public async Task AssignRealmRoleToUserAsync(
        string userId,
        string roleName,
        CancellationToken cancellationToken = default)
    {
        await AssignRealmRolesToUserAsync(userId, new[] { roleName }, cancellationToken);
    }

    public async Task AssignRealmRolesToUserAsync(
        string userId,
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        var allRoles = await GetRealmRolesAsync(cancellationToken);

        var rolesToAssign = allRoles
            .Where(r => roleNames.Contains(r.Name))
            .ToList();

        if (!rolesToAssign.Any())
        {
            return;
        }

        var response = await _httpClient.PostAsJsonAsync(
            $"users/{userId}/role-mappings/realm",
            rolesToAssign,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveRealmRolesFromUserAsync(
        string userId,
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        var allRoles = await GetRealmRolesAsync(cancellationToken);

        var rolesToRemove = allRoles
            .Where(r => roleNames.Contains(r.Name))
            .ToList();

        if (!rolesToRemove.Any())
        {
            return;
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, $"users/{userId}/role-mappings/realm")
        {
            Content = JsonContent.Create(rolesToRemove)
        };

        var response = await _httpClient.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task<List<string>> GetUserDirectRoleNamesAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"users/{userId}/role-mappings/realm", cancellationToken);

        response.EnsureSuccessStatusCode();

        var roles = await response.Content.ReadFromJsonAsync<List<RoleRepresentationModel>>(cancellationToken);

        return roles?.Select(r => r.Name).ToList() ?? new List<string>();
    }

    // Group Management
    public async Task<List<GroupDto>> GetAllGroupsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("groups", cancellationToken);

        response.EnsureSuccessStatusCode();

        var groups = await response.Content.ReadFromJsonAsync<List<GroupRepresentationModel>>(cancellationToken);

        return groups?.Select(g => new GroupDto(
            Guid.Parse(g.Id!),
            g.Name!,
            g.Attributes?.ContainsKey("description") == true
                ? g.Attributes["description"].FirstOrDefault()
                : null,
            string.IsNullOrEmpty(g.ParentId) ? null : Guid.Parse(g.ParentId)
        )).ToList() ?? new List<GroupDto>();
    }

    public async Task<List<GroupDto>> SearchGroupsAsync(
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var url = string.IsNullOrWhiteSpace(searchTerm)
            ? "groups"
            : $"groups?search={Uri.EscapeDataString(searchTerm)}";

        var response = await _httpClient.GetAsync(url, cancellationToken);

        response.EnsureSuccessStatusCode();

        var groups = await response.Content.ReadFromJsonAsync<List<GroupRepresentationModel>>(cancellationToken);

        return groups?.Select(g => new GroupDto(
            Guid.Parse(g.Id!),
            g.Name!,
            g.Attributes?.ContainsKey("description") == true
                ? g.Attributes["description"].FirstOrDefault()
                : null,
            string.IsNullOrEmpty(g.ParentId) ? null : Guid.Parse(g.ParentId)
        )).ToList() ?? new List<GroupDto>();
    }

    public async Task<GroupDto?> GetGroupByIdAsync(
        Guid groupId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"groups/{groupId}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var group = await response.Content.ReadFromJsonAsync<GroupRepresentationModel>(cancellationToken);

            if (group == null) return null;

            return new GroupDto(
                Guid.Parse(group.Id!),
                group.Name!,
                group.Attributes?.ContainsKey("description") == true
                    ? group.Attributes["description"].FirstOrDefault()
                    : null,
                string.IsNullOrEmpty(group.ParentId) ? null : Guid.Parse(group.ParentId)
            );
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<List<UserDto>> GetGroupMembersAsync(
        Guid groupId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"groups/{groupId}/members", cancellationToken);

        response.EnsureSuccessStatusCode();

        var users = await response.Content.ReadFromJsonAsync<List<UserRepresentationModel>>(cancellationToken);

        return users?.Select(u => new UserDto(
            u.Id!,
            u.Username!,
            u.Email!,
            u.FirstName,
            u.LastName
        )).ToList() ?? new List<UserDto>();
    }

    public async Task<List<GroupDto>> GetUserGroupsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"users/{userId}/groups", cancellationToken);

        response.EnsureSuccessStatusCode();

        var groups = await response.Content.ReadFromJsonAsync<List<GroupRepresentationModel>>(cancellationToken);

        return groups?.Select(g => new GroupDto(
            Guid.Parse(g.Id!),
            g.Name!,
            g.Attributes?.ContainsKey("description") == true
                ? g.Attributes["description"].FirstOrDefault()
                : null,
            string.IsNullOrEmpty(g.ParentId) ? null : Guid.Parse(g.ParentId)
        )).ToList() ?? new List<GroupDto>();
    }

    public async Task<Guid> CreateGroupAsync(
        string name,
        string? description,
        CancellationToken cancellationToken = default)
    {
        var group = new GroupRepresentationModel
        {
            Name = name,
            Attributes = description != null
                ? new Dictionary<string, List<string>> { { "description", new List<string> { description } } }
                : null
        };

        var response = await _httpClient.PostAsJsonAsync("groups", group, cancellationToken);

        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location?.ToString() ??
                      throw new InvalidOperationException("Group creation failed - no location header");

        var groupId = location.Split('/').Last();

        return Guid.Parse(groupId);
    }

    public async Task UpdateGroupAsync(
        Guid keycloakGroupId,
        string name,
        string? description,
        CancellationToken cancellationToken = default)
    {
        var group = new GroupRepresentationModel
        {
            Name = name,
            Attributes = description != null
                ? new Dictionary<string, List<string>> { { "description", new List<string> { description } } }
                : null
        };

        var response = await _httpClient.PutAsJsonAsync($"groups/{keycloakGroupId}", group, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteGroupAsync(
        Guid keycloakGroupId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"groups/{keycloakGroupId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return;
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task AddUserToGroupAsync(
        string identityId,
        Guid keycloakGroupId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsync(
            $"users/{identityId}/groups/{keycloakGroupId}",
            null,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveUserFromGroupAsync(
        string identityId,
        Guid keycloakGroupId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(
            $"users/{identityId}/groups/{keycloakGroupId}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return;
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task AssignRolesToGroupAsync(
        Guid keycloakGroupId,
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        var allRoles = await GetRealmRolesAsync(cancellationToken);

        var rolesToAssign = allRoles
            .Where(r => roleNames.Contains(r.Name))
            .ToList();

        if (!rolesToAssign.Any())
        {
            return;
        }

        var response = await _httpClient.PostAsJsonAsync(
            $"groups/{keycloakGroupId}/role-mappings/realm",
            rolesToAssign,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveRolesFromGroupAsync(
        Guid keycloakGroupId,
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        var allRoles = await GetRealmRolesAsync(cancellationToken);

        var rolesToRemove = allRoles
            .Where(r => roleNames.Contains(r.Name))
            .ToList();

        if (!rolesToRemove.Any())
        {
            return;
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, $"groups/{keycloakGroupId}/role-mappings/realm")
        {
            Content = JsonContent.Create(rolesToRemove)
        };

        var response = await _httpClient.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<string>> GetGroupRoleNamesAsync(
        Guid keycloakGroupId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"groups/{keycloakGroupId}/role-mappings/realm", cancellationToken);

        response.EnsureSuccessStatusCode();

        var roles = await response.Content.ReadFromJsonAsync<List<RoleRepresentationModel>>(cancellationToken);

        return roles?.Select(r => r.Name).ToList() ?? new List<string>();
    }

    private async Task<List<RoleRepresentationModel>> GetRealmRolesAsync(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync("roles", cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<RoleRepresentationModel>>(cancellationToken) ??
               new List<RoleRepresentationModel>();
    }
}
