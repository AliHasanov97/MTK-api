namespace MTK.Modules.Hr.Application.FileAttachments.DownloadFileAttachment;

public sealed class DownloadFileAttachmentResponse
{
    public Stream FileStream { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
}
