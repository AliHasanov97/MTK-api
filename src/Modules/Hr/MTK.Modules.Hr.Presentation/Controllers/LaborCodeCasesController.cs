using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.LaborCodeCases.SearchLaborCodeCases;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class LaborCodeCasesController : BaseController
{
    public LaborCodeCasesController(ISender sender) : base(sender)
    {
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchLaborCodeCasesQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }
}
