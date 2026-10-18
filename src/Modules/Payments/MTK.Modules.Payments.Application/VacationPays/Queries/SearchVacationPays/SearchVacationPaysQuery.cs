using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;
using MTK.Modules.Payments.Domain.VacationPays;

namespace MTK.Modules.Payments.Application.VacationPays.Queries.SearchVacationPays;

public sealed record SearchVacationPaysQuery(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize) : IQuery<SearchVacationPaysResponse>;
