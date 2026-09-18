using Microsoft.AspNetCore.Authorization;

namespace MTK.Common.Application.Authorization;

/// <summary>
/// Requires user to have ALL specified roles (AND logic)
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class RequireRoleAttribute : AuthorizeAttribute
{
    public RequireRoleAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
        Policy = $"RequireRole:{string.Join(",", roles)}";
    }
}
