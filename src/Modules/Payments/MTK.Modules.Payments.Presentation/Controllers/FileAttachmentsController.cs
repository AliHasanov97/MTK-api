using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Abstractions;
using MTK.Modules.Payments.Application.FileAttachments.Commands.DeleteFileAttachment;
using MTK.Modules.Payments.Application.FileAttachments.Commands.UploadFileAttachment;
using MTK.Modules.Payments.Application.FileAttachments.Queries.DownloadFileAttachment;
using MTK.Modules.Payments.Application.FileAttachments.Queries.GetFileAttachmentDownloadLink;
using MTK.Modules.Payments.Application.FileAttachments.Queries.ListFileAttachments;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class FileAttachmentsController(ISender sender, ICurrentUserProvider currentUser) : BaseController(sender)
{
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] ListFileAttachmentsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query with { RequestedByUserId = currentUser.UserId }, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager, Roles.Accountant)]
    public async Task<IActionResult> Upload(
        [FromForm] UploadFileAttachmentCommand command,
        CancellationToken cancellationToken)
    {
        // UploadedByUserId always comes from the authenticated request, never the
        // client's form body — overwritten here regardless of what was bound above.
        var result = await _sender.Send(command with { UploadedByUserId = currentUser.UserId }, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Fayl uğurla yükləndi")
            : BadRequest(result.Error);
    }

    [HttpDelete("{fileAttachmentId:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager, Roles.Accountant)]
    public async Task<IActionResult> Delete(Guid fileAttachmentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new DeleteFileAttachmentCommand(fileAttachmentId, currentUser.UserId),
            cancellationToken);

        return result.IsSuccess
            ? Success("Fayl silindi")
            : BadRequest(result.Error);
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

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }
}
