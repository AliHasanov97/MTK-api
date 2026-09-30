using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Payments.Application.VendorCharges.Queries.SearchVendorCharges;

public sealed record SearchVendorChargesQuery(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize) : IQuery<SearchVendorChargesResponse>;
