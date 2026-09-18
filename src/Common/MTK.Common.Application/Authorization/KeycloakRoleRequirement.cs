using Microsoft.AspNetCore.Authorization;

namespace MTK.Common.Application.Authorization;

/// <summary>
/// Authorization requirement for Keycloak role validation
/// </summary>
public sealed class KeycloakRoleRequirement : IAuthorizationRequirement
{
    public KeycloakRoleRequirement(string[] requiredRoles, RoleAuthorizationMode mode)
    {
        RequiredRoles = requiredRoles;
        Mode = mode;
    }

    public string[] RequiredRoles { get; }
    public RoleAuthorizationMode Mode { get; }
}
