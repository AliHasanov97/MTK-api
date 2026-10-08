using MTK.Modules.Payments.Domain.Purchases;

namespace MTK.Modules.Payments.Application.Purchases.Queries.SearchPurchases;

public sealed record SearchPurchasesResponse(
    IReadOnlyCollection<PurchaseListItem> Purchases,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record PurchaseListItem(
    Guid Id,
    Guid VendorId,
    string? VendorName,
    DateTimeOffset PurchaseDate,
    string? InvoiceNumber,
    PurchaseStatus Status,
    decimal TotalAmount,
    DateTimeOffset? ReceivedOnUtc,
    DateTimeOffset CreatedAt);
