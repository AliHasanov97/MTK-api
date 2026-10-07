using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.Queries.SearchNomenclatures;

public sealed record SearchNomenclaturesQuery(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize) : IQuery<SearchNomenclaturesResponse>;
