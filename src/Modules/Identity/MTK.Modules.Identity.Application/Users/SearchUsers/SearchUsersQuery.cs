using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Users.SearchUsers;

public sealed record SearchUsersQuery(
    string? SearchTerm,
    int PageNumber,
    int PageSize,
    string? SortBy,
    string? SortDirection) : IQuery<SearchUsersResponse>;
