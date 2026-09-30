using MTK.Modules.Payments.Domain.Vendors;

namespace MTK.Modules.Payments.Application.Vendors.Queries.GetVendorById;

public sealed record VendorResponse(
    Guid Id,
    string Name,
    VendorType VendorType,
    string? Voen,
    string? Director,
    string? Email,
    string? Phone,
    string? Address,
    string? Note,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
