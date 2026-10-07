using MTK.Common.Application.Messaging;

namespace MTK.Modules.Warehouse.Application.Stocks.Queries.GetLowStockItems;

public sealed record GetLowStockItemsQuery : IQuery<List<LowStockDto>>;

public sealed record LowStockDto(
    Guid NomenclatureId,
    string NomenclatureCode,
    string NomenclatureName,
    decimal QuantityOnHand,
    decimal MinStockLevel,
    decimal Deficit);
