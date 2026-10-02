namespace MTK.Common.Application.Authorization;

/// <summary>
/// Broad Keycloak realm roles, as opposed to <see cref="Permissions"/>'s
/// fine-grained permission strings (which today only the Identity module's
/// user-management endpoints actually check). Buildings and Payments use
/// these simpler role names directly with <see cref="RequireAnyRoleAttribute"/>.
/// </summary>
public static class Roles
{
    public const string Admin = "admin";
    public const string BuildingManager = "building-manager";
    public const string Accountant = "accountant";
    public const string Owner = "owner";
    public const string Employee = "employee";
}
