using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.WarehouseStock.Events;

public sealed record StockIncreasedDomainEvent(
    Guid NomenclatureId,
    decimal Quantity,
    decimal NewQuantityOnHand) : DomainEvent;
