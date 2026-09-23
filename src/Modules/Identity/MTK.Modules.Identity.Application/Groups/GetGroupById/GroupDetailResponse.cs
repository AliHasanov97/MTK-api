namespace MTK.Modules.Identity.Application.Groups.GetGroupById;

public sealed record GroupDetailResponse(
    Guid Id,
    string Name,
    string? Description);
