using MTK.Common.Application.EventBus;

namespace MTK.Modules.Payments.IntegrationEvents.Warehouse;

/// <summary>
/// Alış qəbul edildikdə Warehouse moduluna göndərilir — bu modulda məhsullar alınır,
/// anbarda stok isə bu event ilə artırılır. Bir event bütöv alışı (bütün sətirləri)
/// daşıyır ki, Warehouse tərəfi onu tək tranzaksiyada, atomik şəkildə yaza bilsin.
/// </summary>
public sealed class GoodsReceivedIntegrationEvent : IntegrationEvent
{
    public GoodsReceivedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid purchaseId,
        Guid vendorId,
        DateTimeOffset receivedOnUtc,
        IReadOnlyList<GoodsReceivedLine> lines)
        : base(integrationEventId, occurredOnUtc)
    {
        PurchaseId = purchaseId;
        VendorId = vendorId;
        ReceivedOnUtc = receivedOnUtc;
        Lines = lines;
    }

    public Guid PurchaseId { get; }
    public Guid VendorId { get; }
    public DateTimeOffset ReceivedOnUtc { get; }

    public IReadOnlyList<GoodsReceivedLine> Lines { get; }
}

/// <summary>
/// Alışın bir sətri. <paramref name="NomenclatureId"/> Warehouse-dakı Nomenclature.Id-dir
/// (Payments-dəki güzgünün Id-si onunla eynidir).
/// </summary>
public sealed record GoodsReceivedLine(
    Guid NomenclatureId,
    decimal Quantity,
    decimal? UnitPrice);
