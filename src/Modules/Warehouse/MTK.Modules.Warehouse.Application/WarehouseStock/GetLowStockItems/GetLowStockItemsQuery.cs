using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Application.WarehouseStock.GetLowStockItems;

public sealed record GetLowStockItemsQuery : IQuery<Result<List<LowStockDto>>>;

public sealed record LowStockDto(
    Guid NomenclatureId,
    string NomenclatureCode,
    string NomenclatureName,
    decimal QuantityOnHand,
    decimal MinStockLevel,
    decimal Deficit);
