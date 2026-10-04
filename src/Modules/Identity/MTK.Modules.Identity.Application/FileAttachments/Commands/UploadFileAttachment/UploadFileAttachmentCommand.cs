using Microsoft.AspNetCore.Http;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.FileAttachments.Commands.UploadFileAttachment;

public sealed record UploadFileAttachmentCommand(IFormFile File, Guid UserId) : ICommand<Guid>;
