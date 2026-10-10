using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.UnexcusedAbsences.AddUnexcusedAbsence;
using MTK.Modules.Hr.Application.UnexcusedAbsences.DeleteUnexcusedAbsence;
using MTK.Modules.Hr.Application.UnexcusedAbsences.ExportUnexcusedAbsencePdf;
using MTK.Modules.Hr.Application.UnexcusedAbsences.GetUnexcusedAbsenceById;
using MTK.Modules.Hr.Application.UnexcusedAbsences.SearchUnexcusedAbsences;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class UnexcusedAbsencesController : BaseController
{
    public UnexcusedAbsencesController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddUnexcusedAbsenceCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteUnexcusedAbsenceCommand { Id = id }, cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUnexcusedAbsenceByIdQuery { Id = id }, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchUnexcusedAbsencesQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ExportUnexcusedAbsencePdfQuery(id),
            cancellationToken);

        if (result.IsSuccess)
        {
            var response = result.Value;
            return File(response.FileStream, "application/pdf", response.FileName);
        }

        return NotFound(result.Error);
    }
}
