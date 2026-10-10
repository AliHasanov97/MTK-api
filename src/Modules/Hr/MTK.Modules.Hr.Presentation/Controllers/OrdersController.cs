using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.Orders.SearchOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class OrdersController : BaseController
{
    public OrdersController(ISender sender) : base(sender)
    {
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchOrdersQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }
}