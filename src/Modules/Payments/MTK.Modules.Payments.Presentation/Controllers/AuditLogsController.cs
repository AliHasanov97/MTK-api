using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Payments.Application.AuditLogs.SearchAuditLogs;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class AuditLogsController(ISender sender) : BaseController(sender)
{
    [HttpPost("search")]
    public async Task<IActionResult> SearchAuditLogs(
        [FromBody] SearchAuditLogsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }
}

