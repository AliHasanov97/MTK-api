using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Buildings.Application.FileAttachments.Commands.DeleteFileAttachment;
using MTK.Modules.Buildings.Application.FileAttachments.Commands.UploadFileAttachment;
using MTK.Modules.Buildings.Application.FileAttachments.Queries.DownloadFileAttachment;
using MTK.Modules.Buildings.Application.FileAttachments.Queries.GetFileAttachmentDownloadLink;
using MTK.Modules.Buildings.Application.FileAttachments.Queries.ListFileAttachments;

namespace MTK.Modules.Buildings.Presentation.Controllers;

public class FileAttachmentsController(ISender sender) : BaseController(sender)
{
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] ListFileAttachmentsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> Upload(
        [FromForm] UploadFileAttachmentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Created(result.Value, "Fayl uğurla yükləndi")
            : BadRequest(result.Error);
    }

    [HttpDelete("{fileAttachmentId:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> Delete(Guid fileAttachmentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteFileAttachmentCommand(fileAttachmentId), cancellationToken);

        return result.IsSuccess ? Success("Fayl silindi") : BadRequest(result.Error);
    }

    [HttpGet("{fileAttachmentId:guid}/download")]
    public async Task<IActionResult> Download(Guid fileAttachmentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DownloadFileAttachmentQuery(fileAttachmentId), cancellationToken);

        return result.IsSuccess
            ? File(result.Value.FileContent, result.Value.ContentType, result.Value.FileName)
            : BadRequest(result.Error);
    }

    [HttpGet("{fileAttachmentId:guid}/link")]
    public async Task<IActionResult> GetDownloadLink(Guid fileAttachmentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetFileAttachmentDownloadLinkQuery(fileAttachmentId), cancellationToken);

        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }
}
