using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Payments.Application.Customers.Queries.SearchCustomers;

public sealed record SearchCustomersQuery(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize) : IQuery<SearchCustomersResponse>;
