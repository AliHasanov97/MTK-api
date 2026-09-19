namespace MTK.Modules.Identity.Application.Users.GetUserRoles;

public sealed record UserRolesResponse(
    Guid UserId,
    List<RoleDto> DirectRoles,
    List<RoleDto>? InheritedRoles);

public sealed record RoleDto(
    Guid Id,
    string Name,
    string? Description,
    string RoleType);
