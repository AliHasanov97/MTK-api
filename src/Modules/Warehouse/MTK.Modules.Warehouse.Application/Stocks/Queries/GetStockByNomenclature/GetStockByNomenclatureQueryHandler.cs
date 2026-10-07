using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.WarehouseStock;

namespace MTK.Modules.Warehouse.Application.Stocks.Queries.GetStockByNomenclature;

internal sealed class GetStockByNomenclatureQueryHandler
    : IQueryHandler<GetStockByNomenclatureQuery, StockResponse>
{
    private readonly IWarehouseStockRepository _stockRepository;
    private readonly IMapper _mapper;

    public GetStockByNomenclatureQueryHandler(
        IWarehouseStockRepository stockRepository,
        IMapper mapper)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
    }

    public async Task<Result<StockResponse>> Handle(
        GetStockByNomenclatureQuery request,
        CancellationToken cancellationToken)
    {
        var stock = await _stockRepository.GetByNomenclatureIdAsync(request.NomenclatureId, cancellationToken);

        if (stock is null)
        {
            return Result.Failure<StockResponse>(WarehouseStockErrors.NotFound(request.NomenclatureId));
        }

        var response = _mapper.Map<StockResponse>(stock);

        return Result.Success(response);
    }
}
