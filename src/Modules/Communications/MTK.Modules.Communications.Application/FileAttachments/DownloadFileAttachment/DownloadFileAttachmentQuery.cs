using MTK.Common.Application.Messaging;

namespace MTK.Modules.Communications.Application.FileAttachments.DownloadFileAttachment;

public sealed record DownloadFileAttachmentQuery(Guid Id) : IQuery<DownloadFileAttachmentResponse>;

public sealed class DownloadFileAttachmentResponse
{
    public Stream FileStream { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public bool IsPublic { get; init; }
}
