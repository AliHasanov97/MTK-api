using MTK.Common.Domain.Queries;

namespace MTK.Modules.Buildings.Application.Apartments.Queries.SearchApartments;

public sealed record SearchApartmentsRequest(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize);
