using AutoMapper;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.WarehouseStock;

namespace MTK.Modules.Warehouse.Application.WarehouseStock.GetAllStock;

internal sealed class GetAllStockQueryHandler
    : IQueryHandler<GetAllStockQuery, Result<List<StockDto>>>
{
    private readonly IWarehouseStockRepository _stockRepository;
    private readonly IMapper _mapper;

    public GetAllStockQueryHandler(
        IWarehouseStockRepository stockRepository,
        IMapper mapper)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<StockDto>>> Handle(
        GetAllStockQuery request,
        CancellationToken cancellationToken)
    {
        var stocks = await _stockRepository.GetAllStockAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = _mapper.Map<List<StockDto>>(stocks);

        return Result.Success(dtos);
    }
}
