using MTK.Common.Application.EventBus;

namespace MTK.Modules.Warehouse.IntegrationEvents.WarehouseTransactions;

/// <summary>
/// Anbara mal qəbulu qeyd edildikdə (hazırda yalnız Payments-dəki alışın
/// qəbulundan yaranır) digər modullara bildiriş — uçot/hesabat üçün.
/// </summary>
public sealed class WarehouseReceiptRecordedIntegrationEvent : IntegrationEvent
{
    public WarehouseReceiptRecordedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid transactionId,
        Guid nomenclatureId,
        decimal quantity,
        decimal? unitPrice,
        DateTimeOffset transactionDate,
        string? referenceType,
        Guid? referenceId)
        : base(integrationEventId, occurredOnUtc)
    {
        TransactionId = transactionId;
        NomenclatureId = nomenclatureId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TransactionDate = transactionDate;
        ReferenceType = referenceType;
        ReferenceId = referenceId;
    }

    public Guid TransactionId { get; }
    public Guid NomenclatureId { get; }
    public decimal Quantity { get; }
    public decimal? UnitPrice { get; }
    public DateTimeOffset TransactionDate { get; }

    /// <summary>Mənbə istinadı — məs. "Purchase" + alışın Id-si.</summary>
    public string? ReferenceType { get; }
    public Guid? ReferenceId { get; }
}
