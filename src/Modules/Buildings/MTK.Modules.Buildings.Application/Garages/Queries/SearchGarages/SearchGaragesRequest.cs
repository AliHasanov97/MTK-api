using MTK.Common.Domain.Queries;

namespace MTK.Modules.Buildings.Application.Garages.Queries.SearchGarages;

public sealed record SearchGaragesRequest(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize);
