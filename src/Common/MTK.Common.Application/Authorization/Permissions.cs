namespace MTK.Common.Application.Authorization;

/// <summary>
/// Defines all permissions available in the system
/// </summary>
public static class Permissions
{
    // System Administration
    public const string Admin = "admin";

    // User Management
    public const string UserManagement = "user-management";
    public const string UserCreate = "user-create";
    public const string UserUpdate = "user-update";
    public const string UserDelete = "user-delete";
    public const string UserView = "user-view";

    // Role Management
    public const string RoleManagement = "role-management";
    public const string RoleAssign = "role-assign";
}
