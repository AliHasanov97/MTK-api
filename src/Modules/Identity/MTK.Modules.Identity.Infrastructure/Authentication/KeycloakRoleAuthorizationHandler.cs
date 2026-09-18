using Microsoft.AspNetCore.Authorization;
using MTK.Common.Application.Authorization;
using System.Security.Claims;

namespace MTK.Modules.Identity.Infrastructure.Authentication;

/// <summary>
/// Handles authorization based on Keycloak roles
/// </summary>
public sealed class KeycloakRoleAuthorizationHandler : AuthorizationHandler<KeycloakRoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        KeycloakRoleRequirement requirement)
    {
        var userRoles = GetUserRoles(context.User);

        bool success = requirement.Mode switch
        {
            RoleAuthorizationMode.All => requirement.RequiredRoles.All(role =>
                userRoles.Contains(role)),
            RoleAuthorizationMode.Any => requirement.RequiredRoles.Any(role =>
                userRoles.Contains(role)),
            _ => false
        };

        if (success)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private static HashSet<string> GetUserRoles(ClaimsPrincipal user)
    {
        var roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Get roles from standard ClaimTypes.Role
        foreach (var claim in user.FindAll(ClaimTypes.Role))
        {
            if (!string.IsNullOrEmpty(claim.Value))
            {
                roles.Add(claim.Value);
            }
        }

        // Get roles from "role" claim
        foreach (var claim in user.FindAll("role"))
        {
            if (!string.IsNullOrEmpty(claim.Value))
            {
                roles.Add(claim.Value);
            }
        }

        return roles;
    }
}
