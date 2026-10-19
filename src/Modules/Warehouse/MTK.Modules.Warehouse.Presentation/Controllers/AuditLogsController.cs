using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Warehouse.Application.AuditLogs.SearchAuditLogs;

namespace MTK.Modules.Warehouse.Presentation.Controllers;

// Audit qeydləri yalnız administrator üçündür.
[RequireAnyRole(Roles.Admin)]
public class AuditLogsController(ISender sender) : BaseController(sender)
{
    [HttpPost("search")]
    public async Task<IActionResult> SearchAuditLogs(
        [FromBody] SearchAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(request, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }
}
