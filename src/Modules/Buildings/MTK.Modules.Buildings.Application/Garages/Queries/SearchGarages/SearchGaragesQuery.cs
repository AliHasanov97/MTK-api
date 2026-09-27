using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Buildings.Application.Garages.Queries.SearchGarages;

public sealed record SearchGaragesQuery(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize) : IQuery<SearchGaragesResponse>;
