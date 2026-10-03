using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Roles.SearchRoles;

public sealed record SearchRolesQuery(
    string? SearchTerm,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? SortDirection = null) : IQuery<SearchRolesResponse>;
