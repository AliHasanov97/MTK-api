using Microsoft.AspNetCore.Http;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.FileAttachments.Commands.UploadFileAttachment;

public sealed record UploadFileAttachmentCommand(
    IFormFile File,
    Guid? ContractId,
    Guid? VendorId,
    Guid? PaymentId,
    Guid? OwnerId,
    Guid? PurchaseId,
    Guid? TransactionId,
    // Resolved server-side from the authenticated request (see FileAttachmentsController) —
    // never bound from the client's form data, so a client-supplied value here is
    // always overwritten before the command is sent.
    Guid UploadedByUserId = default) : ICommand<Guid>;
