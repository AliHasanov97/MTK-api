using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.FileAttachments.Commands.DeleteFileAttachment;

// RequestedByUserId is resolved server-side (see FileAttachmentsController) and
// checked against the attachment's own UploadedByUserId — only the person who
// uploaded a document may delete it.
public sealed record DeleteFileAttachmentCommand(Guid FileAttachmentId, Guid RequestedByUserId) : ICommand;
