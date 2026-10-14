using MTK.Modules.Payments.Domain.Customers;

namespace MTK.Modules.Payments.Application.Customers.Queries.SearchCustomers;

public sealed record SearchCustomersResponse(
    IReadOnlyCollection<CustomerSearchResult> Customers,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record CustomerSearchResult(
    Guid Id,
    string Name,
    CustomerType CustomerType,
    string? Voen,
    string? Director,
    string? Email,
    string? Phone,
    string? Address,
    bool IsActive,
    DateTimeOffset CreatedAt);
