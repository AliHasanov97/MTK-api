using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Groups.SearchGroups;

public sealed record SearchGroupsQuery(
    string? SearchTerm,
    int PageNumber,
    int PageSize,
    string? SortBy,
    string? SortDirection) : IQuery<SearchGroupsResponse>;
