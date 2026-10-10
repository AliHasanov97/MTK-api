using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.AddNoticeOfChangeInWorkingConditions;
using MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.DeleteNoticeOfChangeInWorkingConditions;
using MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.ExportNoticeOfChangeInWorkingConditionsWord;
using MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.GetNoticeOfChangeInWorkingConditionsById;
using MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.SearchNoticesOfChangeInWorkingConditions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class NoticesOfChangeInWorkingConditionsController : BaseController
{
    public NoticesOfChangeInWorkingConditionsController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddNoticeOfChangeInWorkingConditionsCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteNoticeOfChangeInWorkingConditionsCommand { Id = id }, cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetNoticeOfChangeInWorkingConditionsByIdQuery { Id = id }, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchNoticesOfChangeInWorkingConditionsQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}/export-word")]
    public async Task<IActionResult> ExportWord([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ExportNoticeOfChangeInWorkingConditionsWordQuery(id),
            cancellationToken);

        if (result.IsSuccess)
        {
            var response = result.Value;
            return File(response.FileStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", response.FileName);
        }

        return NotFound(result.Error);
    }
}