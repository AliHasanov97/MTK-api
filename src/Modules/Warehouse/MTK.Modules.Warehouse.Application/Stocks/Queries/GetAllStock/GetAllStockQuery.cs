using MTK.Common.Application.Messaging;

namespace MTK.Modules.Warehouse.Application.Stocks.Queries.GetAllStock;

public sealed record GetAllStockQuery(
    int PageNumber = 1,
    int PageSize = 50) : IQuery<List<StockDto>>;

public sealed record StockDto(
    Guid NomenclatureId,
    string NomenclatureCode,
    string NomenclatureName,
    decimal QuantityOnHand,
    DateTimeOffset? LastTransactionDate);
