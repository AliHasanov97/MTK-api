namespace MTK.Modules.Communications.Application.Articles;

public sealed record ArticleResponse(
    Guid Id,
    string Title,
    string Summary,
    string Content,
    string Status,
    DateTimeOffset? PublishedAt,
    DateTimeOffset CreatedAt,
    int ViewCount,
    string CreatedByFullName,
    List<FileAttachmentResponse> FileAttachments);

public sealed record FileAttachmentResponse(
    Guid Id,
    string FileName,
    string MimeType,
    bool IsPublic);
