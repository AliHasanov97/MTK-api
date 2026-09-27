using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Buildings.Application.Owners.Queries.SearchOwners;

public sealed record SearchOwnersQuery(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize) : IQuery<SearchOwnersResponse>;
