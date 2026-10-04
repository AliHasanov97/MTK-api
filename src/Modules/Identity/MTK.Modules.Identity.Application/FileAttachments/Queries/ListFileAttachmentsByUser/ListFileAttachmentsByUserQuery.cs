using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.FileAttachments.Queries.ListFileAttachmentsByUser;

public sealed record ListFileAttachmentsByUserQuery(Guid UserId) : IQuery<List<FileAttachmentResponse>>;

public sealed record FileAttachmentResponse(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTimeOffset CreatedAt);
