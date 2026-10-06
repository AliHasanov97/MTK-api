using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Application.WarehouseStock.GetStockByNomenclature;

public sealed record GetStockByNomenclatureQuery(Guid NomenclatureId) : IQuery<Result<StockResponse>>;

public sealed record StockResponse(
    Guid NomenclatureId,
    string NomenclatureCode,
    string NomenclatureName,
    decimal QuantityOnHand,
    DateTimeOffset? LastTransactionDate,
    decimal? MinStockLevel,
    bool IsLowStock);
