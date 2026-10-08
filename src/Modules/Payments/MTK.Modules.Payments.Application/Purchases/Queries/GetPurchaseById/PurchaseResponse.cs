using MTK.Modules.Payments.Domain.Purchases;

namespace MTK.Modules.Payments.Application.Purchases.Queries.GetPurchaseById;

public sealed record PurchaseResponse(
    Guid Id,
    Guid VendorId,
    string? VendorName,
    DateTimeOffset PurchaseDate,
    string? InvoiceNumber,
    string? Note,
    PurchaseStatus Status,
    DateTimeOffset? ReceivedOnUtc,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    IReadOnlyCollection<PurchaseLineResponse> Lines);

public sealed record PurchaseLineResponse(
    Guid Id,
    Guid NomenclatureId,
    string? NomenclatureCode,
    string? NomenclatureName,
    decimal Quantity,
    decimal UnitPrice,
    decimal LineTotal);
