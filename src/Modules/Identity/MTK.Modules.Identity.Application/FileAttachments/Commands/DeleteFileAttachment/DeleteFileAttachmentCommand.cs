using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.FileAttachments.Commands.DeleteFileAttachment;

public sealed record DeleteFileAttachmentCommand(Guid FileAttachmentId) : ICommand;
