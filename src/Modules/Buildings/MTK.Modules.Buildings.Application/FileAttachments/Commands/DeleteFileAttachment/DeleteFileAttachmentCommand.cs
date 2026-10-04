using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.FileAttachments.Commands.DeleteFileAttachment;

public sealed record DeleteFileAttachmentCommand(Guid FileAttachmentId) : ICommand;
