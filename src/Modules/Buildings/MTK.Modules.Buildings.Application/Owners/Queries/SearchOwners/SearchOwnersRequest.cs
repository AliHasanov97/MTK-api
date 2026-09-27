using MTK.Common.Domain.Queries;

namespace MTK.Modules.Buildings.Application.Owners.Queries.SearchOwners;

public sealed record SearchOwnersRequest(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize);
