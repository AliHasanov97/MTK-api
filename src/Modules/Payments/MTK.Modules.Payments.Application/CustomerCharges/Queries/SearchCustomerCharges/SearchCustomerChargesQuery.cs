using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Payments.Application.CustomerCharges.Queries.SearchCustomerCharges;

public sealed record SearchCustomerChargesQuery(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize) : IQuery<SearchCustomerChargesResponse>;
