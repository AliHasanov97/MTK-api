using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Roles;

public static class RoleErrors
{
    public static Error NotFound(Guid roleId) =>
        new Error("Roles.NotFound", $"The role with Id '{roleId}' was not found");

    public static Error NotFoundByName(string roleName) =>
        new Error("Roles.NotFoundByName", $"The role with name '{roleName}' was not found");

    public static Error AlreadyExists(string roleName) =>
        new Error("Roles.AlreadyExists", $"A role with name '{roleName}' already exists");

    public static Error CannotDelete(Guid roleId) =>
        new Error("Roles.CannotDelete", $"The role with Id '{roleId}' cannot be deleted because it is assigned to users or groups");
}
