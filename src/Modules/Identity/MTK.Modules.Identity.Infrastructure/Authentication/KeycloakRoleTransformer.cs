using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace MTK.Modules.Identity.Infrastructure.Authentication;

public sealed class KeycloakRoleTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        if (identity == null || !identity.IsAuthenticated)
        {
            return Task.FromResult(principal);
        }

        var roles = new HashSet<string>();

        // Extract from realm_access.roles (realm-level roles)
        var realmAccess = principal.FindFirst("realm_access")?.Value;
        if (!string.IsNullOrEmpty(realmAccess))
        {
            try
            {
                var doc = JsonDocument.Parse(realmAccess);
                if (doc.RootElement.TryGetProperty("roles", out var rolesArray))
                {
                    foreach (var role in rolesArray.EnumerateArray())
                    {
                        var roleValue = role.GetString();
                        if (!string.IsNullOrEmpty(roleValue))
                        {
                            roles.Add(roleValue);
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // Ignore JSON parsing errors
            }
        }

        // Extract from resource_access (client-level roles)
        var resourceAccess = principal.FindFirst("resource_access")?.Value;
        if (!string.IsNullOrEmpty(resourceAccess))
        {
            try
            {
                var doc = JsonDocument.Parse(resourceAccess);
                foreach (var client in doc.RootElement.EnumerateObject())
                {
                    if (client.Value.TryGetProperty("roles", out var clientRoles))
                    {
                        foreach (var role in clientRoles.EnumerateArray())
                        {
                            var roleValue = role.GetString();
                            if (!string.IsNullOrEmpty(roleValue))
                            {
                                // Add client-prefixed role (e.g., "mtk-api.admin")
                                roles.Add($"{client.Name}.{roleValue}");
                            }
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // Ignore JSON parsing errors
            }
        }

        // Add roles as claims
        foreach (var role in roles.Distinct())
        {
            // Add both standard ClaimTypes.Role and "role" claim for compatibility
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
            identity.AddClaim(new Claim("role", role));
        }

        return Task.FromResult(principal);
    }
}
