namespace MTK.Modules.Identity.Application.Roles.GetRoleAssignments;

public sealed record RoleAssignmentsResponse(
    Guid RoleId,
    string RoleName,
    List<GroupInfo> Groups,
    int UserCount);

public sealed record GroupInfo(
    Guid Id,
    string Name);
