using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Purchases.Events;

/// <summary>
/// Alış qəbul edildikdə qaldırılır — Warehouse moduluna stok artımı üçün
/// integration event göndərilməsinə səbəb olur.
/// </summary>
public sealed record PurchaseReceivedDomainEvent(
    Guid PurchaseId,
    Guid VendorId,
    DateTimeOffset ReceivedOnUtc) : DomainEvent;
