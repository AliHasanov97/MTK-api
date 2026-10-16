using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Payments.Application.SalaryPayments.Queries.SearchSalaryPayments;

/// <summary>
/// Maaş ödənişlərini axtarış et.
/// </summary>
public sealed record SearchSalaryPaymentsQuery(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize) : IQuery<SearchSalaryPaymentsResponse>;
