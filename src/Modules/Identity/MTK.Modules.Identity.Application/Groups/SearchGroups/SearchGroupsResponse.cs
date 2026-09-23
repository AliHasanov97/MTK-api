namespace MTK.Modules.Identity.Application.Groups.SearchGroups;

public sealed record SearchGroupsResponse(
    List<GroupSearchResult> Groups,
    int TotalCount,
    int PageNumber,
    int PageSize);

public sealed record GroupSearchResult(
    Guid Id,
    string Name,
    string? Description,
    Guid? ParentId);
