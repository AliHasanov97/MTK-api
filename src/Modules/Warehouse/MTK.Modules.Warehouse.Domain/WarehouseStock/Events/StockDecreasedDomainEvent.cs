using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.WarehouseStock.Events;

public sealed record StockDecreasedDomainEvent(
    Guid NomenclatureId,
    decimal Quantity,
    decimal NewQuantityOnHand) : DomainEvent;
