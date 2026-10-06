using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.WarehouseTransactions.Events;

public sealed record WarehouseIssueRecordedDomainEvent(
    Guid TransactionId,
    Guid NomenclatureId,
    decimal Quantity) : DomainEvent;
