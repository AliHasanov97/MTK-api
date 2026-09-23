namespace MTK.Modules.Identity.Application.Groups.GetGroups;

public sealed record GroupResponse(
    Guid Id,
    string Name,
    string? Description);
