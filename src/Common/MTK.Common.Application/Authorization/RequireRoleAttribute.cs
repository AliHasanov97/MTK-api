using Microsoft.AspNetCore.Authorization;

namespace MTK.Common.Application.Authorization;

/// <summary>
/// Specifies that access to a controller or action requires ALL of the specified roles (AND logic).
/// User must have every role listed.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequireRoleAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "RequireRole:";

    /// <summary>
    /// Requires ALL of the specified roles
    /// </summary>
    /// <param name="roles">List of roles - user must have ALL of them</param>
    public RequireRoleAttribute(params string[] roles)
    {
        if (roles == null || roles.Length == 0)
            throw new ArgumentException("At least one role must be specified", nameof(roles));

        Policy = $"{PolicyPrefix}{string.Join(",", roles)}";
    }
}
