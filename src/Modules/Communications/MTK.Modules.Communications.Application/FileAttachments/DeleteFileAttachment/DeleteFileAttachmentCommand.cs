using MTK.Common.Application.Messaging;

namespace MTK.Modules.Communications.Application.FileAttachments.DeleteFileAttachment;

public sealed record DeleteFileAttachmentCommand(Guid Id) : ICommand;
