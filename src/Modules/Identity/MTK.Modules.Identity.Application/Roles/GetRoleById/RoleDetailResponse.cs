namespace MTK.Modules.Identity.Application.Roles.GetRoleById;

public sealed record RoleDetailResponse(
    Guid Id,
    string Name,
    string? Description,
    string RoleType,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
