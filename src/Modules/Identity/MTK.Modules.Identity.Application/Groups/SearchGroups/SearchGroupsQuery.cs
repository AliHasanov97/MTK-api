using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Groups.SearchGroups;

public sealed record SearchGroupsQuery(
    string? SearchTerm,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? SortDirection = null) : IQuery<SearchGroupsResponse>;
