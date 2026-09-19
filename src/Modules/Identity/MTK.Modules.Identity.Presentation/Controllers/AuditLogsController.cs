using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Identity.Application.AuditLogs.SearchAuditLogs;

namespace MTK.Modules.Identity.Presentation.Controllers;

[ApiController]
[Route("api/identity/auditlogs")]
public class AuditLogsController : ControllerBase
{
    private readonly ISender _sender;

    public AuditLogsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("search")]
    [Authorize]
    public async Task<IActionResult> SearchAuditLogs([FromBody] SearchAuditLogsRequest request, CancellationToken cancellationToken)
    {
        var query = new SearchAuditLogsQuery(
            request.EntityType,
            request.EntityId,
            request.Action,
            request.UserId,
            request.DateFrom,
            request.DateTo,
            request.PageNumber,
            request.PageSize);

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }
}

// Request DTO
public sealed record SearchAuditLogsRequest(
    string? EntityType,
    Guid? EntityId,
    string? Action,
    Guid? UserId,
    DateTime? DateFrom,
    DateTime? DateTo,
    int PageNumber = 1,
    int PageSize = 10);
