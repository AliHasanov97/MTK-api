using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Roles.SearchRoles;

public sealed record SearchRolesQuery(
    string? SearchTerm,
    int PageNumber,
    int PageSize,
    string? SortBy,
    string? SortDirection) : IQuery<SearchRolesResponse>;
