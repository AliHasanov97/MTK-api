using MTK.Common.Application.Messaging;

namespace MTK.Modules.Warehouse.Application.Stocks.Queries.GetStockByNomenclature;

public sealed record GetStockByNomenclatureQuery(Guid NomenclatureId) : IQuery<StockResponse>;

public sealed record StockResponse(
    Guid NomenclatureId,
    string NomenclatureCode,
    string NomenclatureName,
    decimal QuantityOnHand,
    DateTimeOffset? LastTransactionDate,
    decimal? MinStockLevel,
    bool IsLowStock);
