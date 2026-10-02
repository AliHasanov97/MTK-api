using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Domain.Queries;
using MTK.Modules.Buildings.Application.AuditLogs.SearchAuditLogs;

namespace MTK.Modules.Buildings.Presentation.Controllers;

public class AuditLogsController(ISender sender) : BaseController(sender)
{
    [HttpPost("search")]
    public async Task<IActionResult> SearchAuditLogs([FromBody] SearchAuditLogsQuery request,
        CancellationToken cancellationToken)
    {

        var query = new SearchAuditLogsQuery(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize);

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Success(result.Value);
    }
}