using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.WarehouseTransactions.Events;

public sealed record WarehouseReceiptRecordedDomainEvent(
    Guid TransactionId,
    Guid NomenclatureId,
    decimal Quantity) : DomainEvent;
