namespace MTK.Modules.Identity.Application.Users.GetUserRolesAndGroups;

public sealed record UserRolesAndGroupsResponse(
    Guid UserId,
    List<RoleInfo> DirectRoles,
    List<RoleInfo> InheritedRoles,
    List<GroupInfo> Groups);

public sealed record RoleInfo(
    Guid Id,
    string Name,
    string? Description,
    string RoleType);

public sealed record GroupInfo(
    Guid Id,
    Guid KeycloakGroupId,
    string Name,
    string? Description);
