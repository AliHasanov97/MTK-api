using MTK.Common.Application.Messaging;

namespace MTK.Modules.Warehouse.Application.Transactions.Commands.RecordReceiptFromPurchase;

/// <summary>
/// Payments modulunda qəbul edilmiş alışı anbara yazır — hər sətir üçün mal qəbulu
/// əməliyyatı yaradılır və stok artırılır. Idempotentdir: eyni alış üçün əməliyyat
/// artıq varsa, heç nə etmir.
/// </summary>
public sealed record RecordReceiptFromPurchaseCommand(
    Guid PurchaseId,
    DateTimeOffset ReceivedOnUtc,
    List<ReceivedLine> Lines) : ICommand;

public sealed record ReceivedLine(
    Guid NomenclatureId,
    decimal Quantity,
    decimal? UnitPrice);
