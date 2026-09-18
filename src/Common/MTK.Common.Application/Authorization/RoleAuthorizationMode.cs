namespace MTK.Common.Application.Authorization;

public enum RoleAuthorizationMode
{
    /// <summary>
    /// User must have ALL specified roles (AND logic)
    /// </summary>
    All,

    /// <summary>
    /// User must have AT LEAST ONE of the specified roles (OR logic)
    /// </summary>
    Any
}
