using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Warehouse.Application.Stocks.Queries.GetAllStock;
using MTK.Modules.Warehouse.Application.Stocks.Queries.GetLowStockItems;
using MTK.Modules.Warehouse.Application.Stocks.Queries.GetStockByNomenclature;

namespace MTK.Modules.Warehouse.Presentation.Controllers;

public class StocksController(ISender sender) : BaseController(sender)
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllStockQuery(pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock(CancellationToken cancellationToken)
    {
        var query = new GetLowStockItemsQuery();
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("nomenclature/{nomenclatureId:guid}")]
    public async Task<IActionResult> GetByNomenclature(
        Guid nomenclatureId,
        CancellationToken cancellationToken)
    {
        var query = new GetStockByNomenclatureQuery(nomenclatureId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }
}
