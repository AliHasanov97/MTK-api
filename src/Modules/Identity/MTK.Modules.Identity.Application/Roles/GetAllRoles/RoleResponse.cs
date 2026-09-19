namespace MTK.Modules.Identity.Application.Roles.GetAllRoles;

public sealed record RoleResponse(
    Guid Id,
    string Name,
    string? Description,
    string RoleType,
    bool IsActive);
