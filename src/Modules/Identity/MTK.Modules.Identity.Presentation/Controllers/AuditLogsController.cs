using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Identity.Application.AuditLogs.SearchAuditLogs;

namespace MTK.Modules.Identity.Presentation.Controllers;

[ApiController]
[Route("api/identity/auditlogs")]
public class AuditLogsController(ISender sender) : ControllerBase
{
    [HttpPost("search")]
    [Authorize]
    public async Task<IActionResult> SearchAuditLogs(
        [FromBody] SearchAuditLogsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { data = result.Value, message = "Uğurla icra edildi" });
    }
}