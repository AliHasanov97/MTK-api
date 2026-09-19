namespace MTK.Modules.Identity.Application.Groups.GetGroupById;

public sealed record GroupDetailResponse(
    Guid Id,
    Guid KeycloakGroupId,
    string Name,
    string? Description,
    Guid? ParentGroupId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
