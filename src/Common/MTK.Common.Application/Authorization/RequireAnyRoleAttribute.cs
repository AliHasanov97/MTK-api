using Microsoft.AspNetCore.Authorization;

namespace MTK.Common.Application.Authorization;

/// <summary>
/// Requires user to have AT LEAST ONE of the specified roles (OR logic)
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class RequireAnyRoleAttribute : AuthorizeAttribute
{
    public RequireAnyRoleAttribute(params string[] roles)
    {
        Policy = $"RequireAnyRole:{string.Join(",", roles)}";
    }
}
