using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.WarehouseStock;

namespace MTK.Modules.Warehouse.Application.Stocks.Queries.GetLowStockItems;

internal sealed class GetLowStockItemsQueryHandler
    : IQueryHandler<GetLowStockItemsQuery, List<LowStockDto>>
{
    private readonly IWarehouseStockRepository _stockRepository;

    public GetLowStockItemsQueryHandler(IWarehouseStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<Result<List<LowStockDto>>> Handle(
        GetLowStockItemsQuery request,
        CancellationToken cancellationToken)
    {
        var lowStockItems = await _stockRepository.GetLowStockItemsAsync(cancellationToken);

        var dtos = lowStockItems
            .Where(stock => stock.Nomenclature.MinStockLevel.HasValue)
            .Select(stock => new LowStockDto(
                stock.NomenclatureId,
                stock.Nomenclature.Code,
                stock.Nomenclature.Name,
                stock.QuantityOnHand,
                stock.Nomenclature.MinStockLevel!.Value,
                stock.Nomenclature.MinStockLevel.Value - stock.QuantityOnHand))
            .ToList();

        return Result.Success(dtos);
    }
}
