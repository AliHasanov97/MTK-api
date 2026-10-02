using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.AuditLogs.SearchAuditLogs;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class AuditLogsController(ISender sender) : BaseController(sender)
{
    [HttpPost("search")]
    public async Task<IActionResult> SearchAuditLogs([FromBody] SearchAuditLogsQuery request, CancellationToken cancellationToken)
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
    
    protected IActionResult Success<T>(T data, string message = "Uğurla icra edildi")
    {
        return Ok(new { data, message });
    }

    protected IActionResult Success(string message = "Uğurla icra edildi")
    {
        return Ok(new { message });
    }

    protected IActionResult BadRequest(Error error)
    {
        return base.BadRequest(new { error = error.Message, code = error.Code });
    }
}

