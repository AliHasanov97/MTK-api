using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Payments.Application.Nomenclatures.Queries.SearchNomenclatureShadows;

public sealed record SearchNomenclatureShadowsQuery(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize) : IQuery<SearchNomenclatureShadowsResponse>;
