using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.FileAttachments.AddBulkFileAttachment;
using MTK.Modules.Hr.Application.FileAttachments.AddFileAttachment;
using MTK.Modules.Hr.Application.FileAttachments.DeleteFileAttachment;
using MTK.Modules.Hr.Application.FileAttachments.DownloadFileAttachment;
using MTK.Modules.Hr.Application.FileAttachments.SearchFileAttachments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class FileAttachmentsController(ISender sender) : BaseController(sender)
{
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchFileAttachmentsQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            return Success(result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddFileAttachment([FromForm] AddFileAttachmentCommand command, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(command, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }

        return BadRequest(response.Error);
    }

    [HttpPost("bulk")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddBulkFileAttachment([FromForm] AddBulkFileAttachmentCommand command, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(command, cancellationToken);
        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }

        return BadRequest(response.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteFileAttachment(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteFileAttachmentCommand { Id = id };
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return BadRequest(result.Error);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new DownloadFileAttachmentQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            var response = result.Value;
            return File(response.FileStream, response.ContentType, response.FileName);
        }

        return NotFound(result.Error);
    }
}
