namespace MTK.Modules.Identity.Application.Groups.GetGroups;

public sealed record GroupResponse(
    Guid Id,
    Guid KeycloakGroupId,
    string Name,
    string? Description,
    Guid? ParentGroupId);
