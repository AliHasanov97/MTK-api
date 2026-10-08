using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Purchases.Queries.GetNomenclaturePurchaseHistory;

public sealed record GetNomenclaturePurchaseHistoryQuery(Guid NomenclatureId)
    : IQuery<List<NomenclaturePurchaseHistoryItem>>;

public sealed record NomenclaturePurchaseHistoryItem(
    Guid PurchaseId,
    DateTimeOffset PurchaseDate,
    string? InvoiceNumber,
    string? VendorName,
    Guid VendorId,
    int Status,
    decimal Quantity,
    decimal UnitPrice,
    decimal LineTotal);
