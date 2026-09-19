namespace MTK.Modules.Identity.Application.Roles.SearchRoles;

public sealed record SearchRolesResponse(
    List<RoleSearchResult> Roles,
    int TotalCount,
    int PageNumber,
    int PageSize);

public sealed record RoleSearchResult(
    Guid Id,
    string Name,
    string? Description,
    string RoleType,
    bool IsActive);
