using MTK.Modules.Payments.Domain.Vendors;

namespace MTK.Modules.Payments.Application.Vendors.Queries.SearchVendors;

public sealed record SearchVendorsResponse(
    IReadOnlyCollection<VendorSearchResult> Vendors,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record VendorSearchResult(
    Guid Id,
    string Name,
    VendorType VendorType,
    string? Voen,
    string? Director,
    string? Email,
    string? Phone,
    string? Address,
    bool IsActive,
    DateTimeOffset CreatedAt);
