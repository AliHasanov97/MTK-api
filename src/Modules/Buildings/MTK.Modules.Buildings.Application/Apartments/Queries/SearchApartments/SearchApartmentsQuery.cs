using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Buildings.Application.Apartments.Queries.SearchApartments;

public sealed record SearchApartmentsQuery(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize) : IQuery<SearchApartmentsResponse>;
