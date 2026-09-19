using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Groups;

public static class GroupErrors
{
    public static Error NotFound(Guid groupId) =>
        new Error("Groups.NotFound", $"The group with Id '{groupId}' was not found");

    public static Error NotFoundByKeycloakId(Guid keycloakGroupId) =>
        new Error("Groups.NotFoundByKeycloakId", $"The group with Keycloak ID '{keycloakGroupId}' was not found");

    public static Error AlreadyExists(string groupName) =>
        new Error("Groups.AlreadyExists", $"A group with name '{groupName}' already exists");

    public static Error CannotDelete(Guid groupId) =>
        new Error("Groups.CannotDelete", $"The group with Id '{groupId}' cannot be deleted because it has subgroups or members");

    public static Error CircularReference(Guid groupId, Guid parentGroupId) =>
        new Error("Groups.CircularReference", $"Setting group '{parentGroupId}' as parent would create a circular reference");
}
