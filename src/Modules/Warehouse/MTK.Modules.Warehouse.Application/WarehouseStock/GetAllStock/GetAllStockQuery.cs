using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Application.WarehouseStock.GetAllStock;

public sealed record GetAllStockQuery(
    int PageNumber = 1,
    int PageSize = 50) : IQuery<Result<List<StockDto>>>;

public sealed record StockDto(
    Guid NomenclatureId,
    string NomenclatureCode,
    string NomenclatureName,
    decimal QuantityOnHand,
    DateTimeOffset? LastTransactionDate);
