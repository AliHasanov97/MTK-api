namespace MTK.Modules.Identity.Application.Groups.GetGroupRoles;

public sealed record RoleDto(
    Guid Id,
    string Name,
    string? Description,
    string RoleType);
