using Microsoft.AspNetCore.Authorization;

namespace MTK.Common.Application.Authorization;

/// <summary>
/// Specifies that access to a controller or action requires AT LEAST ONE of the specified roles (OR logic).
/// User needs only one of the roles listed.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequireAnyRoleAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "RequireAnyRole:";

    /// <summary>
    /// Requires AT LEAST ONE of the specified roles
    /// </summary>
    /// <param name="roles">List of roles - user needs at least one</param>
    public RequireAnyRoleAttribute(params string[] roles)
    {
        if (roles == null || roles.Length == 0)
            throw new ArgumentException("At least one role must be specified", nameof(roles));

        Policy = $"{PolicyPrefix}{string.Join(",", roles)}";
    }
}
