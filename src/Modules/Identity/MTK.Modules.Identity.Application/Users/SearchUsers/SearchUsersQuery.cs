using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Users.SearchUsers;

public sealed record SearchUsersQuery(
    string? SearchTerm,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? SortDirection = null) : IQuery<SearchUsersResponse>;
