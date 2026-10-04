using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Identity.Application.FileAttachments.Commands.DeleteFileAttachment;
using MTK.Modules.Identity.Application.FileAttachments.Commands.UploadFileAttachment;
using MTK.Modules.Identity.Application.FileAttachments.Queries.DownloadFileAttachment;
using MTK.Modules.Identity.Application.FileAttachments.Queries.GetFileAttachmentDownloadLink;
using MTK.Modules.Identity.Application.FileAttachments.Queries.ListFileAttachmentsByUser;

namespace MTK.Modules.Identity.Presentation.Controllers;

[ApiController]
[Route("api/identity/file-attachments")]
[Authorize]
public class FileAttachmentsController : ControllerBase
{
    private readonly ISender _sender;

    public FileAttachmentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> ListByUser(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ListFileAttachmentsByUserQuery(userId), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error.Message });
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin)]
    public async Task<IActionResult> Upload([FromForm] UploadFileAttachmentCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(new { data = result.Value, message = "Fayl uğurla yükləndi" })
            : BadRequest(new { error = result.Error.Message });
    }

    [HttpDelete("{fileAttachmentId:guid}")]
    [RequireAnyRole(Roles.Admin)]
    public async Task<IActionResult> Delete(Guid fileAttachmentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteFileAttachmentCommand(fileAttachmentId), cancellationToken);

        return result.IsSuccess
            ? Ok(new { message = "Fayl silindi" })
            : BadRequest(new { error = result.Error.Message });
    }

    [HttpGet("{fileAttachmentId:guid}/download")]
    public async Task<IActionResult> Download(Guid fileAttachmentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DownloadFileAttachmentQuery(fileAttachmentId), cancellationToken);

        return result.IsSuccess
            ? File(result.Value.FileContent, result.Value.ContentType, result.Value.FileName)
            : BadRequest(new { error = result.Error.Message });
    }

    [HttpGet("{fileAttachmentId:guid}/link")]
    public async Task<IActionResult> GetDownloadLink(Guid fileAttachmentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetFileAttachmentDownloadLinkQuery(fileAttachmentId), cancellationToken);

        return result.IsSuccess
            ? Ok(new { url = result.Value })
            : BadRequest(new { error = result.Error.Message });
    }
}
