using MTK.Common.Application.Authorization;
using MTK.Modules.Hr.Application.VacationBalances.Queries.GetVacationBalanceByEmployeeId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class VacationBalancesController : BaseController
{
    public VacationBalancesController(ISender sender) : base(sender)
    {
    }

    [HttpGet("{employeeId:guid}")]
    public async Task<IActionResult> GetByEmployeeId(
        [FromRoute] Guid employeeId,
        [FromQuery] DateTimeOffset? asOfDate,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetVacationBalanceByEmployeeIdQuery(employeeId, asOfDate), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }
}
