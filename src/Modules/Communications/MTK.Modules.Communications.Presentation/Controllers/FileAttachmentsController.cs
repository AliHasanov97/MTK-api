using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Communications.Application.FileAttachments.AddFileAttachment;
using MTK.Modules.Communications.Application.FileAttachments.DeleteFileAttachment;
using MTK.Modules.Communications.Application.FileAttachments.DownloadFileAttachment;

namespace MTK.Modules.Communications.Presentation.Controllers;

public class FileAttachmentsController(ISender sender) : BaseController(sender)
{
    /// <summary>
    /// Xəbərə fayl yüklə (Admin və Komandant). Əvvəlcə xəbər yaradılmalıdır.
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> Add([FromForm] AddFileAttachmentCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value, "Fayl yükləndi") : BadRequest(result.Error);
    }

    /// <summary>
    /// Faylı sil (Admin və Komandant).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteFileAttachmentCommand(id), cancellationToken);
        return result.IsSuccess ? Success("Fayl silindi") : BadRequest(result.Error);
    }

    /// <summary>
    /// Faylı endir. Açıq (IsPublic) fayllar hamıya, daxili fayllar yalnız admin və komandanta.
    /// </summary>
    [HttpGet("{id:guid}/download")]
    [AllowAnonymous]
    public async Task<IActionResult> Download(
        Guid id,
        [FromServices] IAuthorizationService authorizationService,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DownloadFileAttachmentQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        var file = result.Value;
        if (!file.IsPublic)
        {
            var policy = $"{RequireAnyRoleAttribute.PolicyPrefix}{Roles.Admin},{Roles.BuildingManager}";
            var authorization = await authorizationService.AuthorizeAsync(User, null, policy);
            if (!authorization.Succeeded)
            {
                await file.FileStream.DisposeAsync();
                return User.Identity?.IsAuthenticated == true ? Forbid() : Unauthorized();
            }
        }

        return File(file.FileStream, file.ContentType, file.FileName);
    }
}
