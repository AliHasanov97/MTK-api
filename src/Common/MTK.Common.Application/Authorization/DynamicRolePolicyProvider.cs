using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace MTK.Common.Application.Authorization;

/// <summary>
/// Provides dynamic authorization policies for role-based access control
/// </summary>
public sealed class DynamicRolePolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public DynamicRolePolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => _fallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        => _fallbackPolicyProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Handle RequireRole:{role1},{role2} - ALL roles required (AND)
        if (policyName.StartsWith("RequireRole:", StringComparison.OrdinalIgnoreCase))
        {
            var roles = policyName["RequireRole:".Length..].Split(',');
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new KeycloakRoleRequirement(roles, RoleAuthorizationMode.All))
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        // Handle RequireAnyRole:{role1},{role2} - AT LEAST ONE role required (OR)
        if (policyName.StartsWith("RequireAnyRole:", StringComparison.OrdinalIgnoreCase))
        {
            var roles = policyName["RequireAnyRole:".Length..].Split(',');
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new KeycloakRoleRequirement(roles, RoleAuthorizationMode.Any))
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        // Fallback to default policy provider
        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}
