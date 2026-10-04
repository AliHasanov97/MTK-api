using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.FileAttachments.Queries.ListFileAttachments;

/// <summary>Düz bir əlaqəli obyekt göstərilir — digərləri boş saxlanılır.</summary>
public sealed record ListFileAttachmentsQuery(
    Guid? ContractId = null,
    Guid? VendorId = null,
    Guid? PaymentId = null,
    Guid? OwnerId = null,
    Guid? TransactionId = null,
    // Resolved server-side from the authenticated request (see FileAttachmentsController) —
    // never bound from the client's query string, so a client-supplied value here is
    // always overwritten before the query is sent.
    Guid RequestedByUserId = default) : IQuery<List<FileAttachmentResponse>>;

public sealed record FileAttachmentResponse(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTimeOffset CreatedAt,
    // Whoever uploaded a document may delete it again — everyone else (incl.
    // residents) is download-only. Computed server-side so the frontend never
    // needs to know its own user id or anyone else's.
    bool CanDelete);
