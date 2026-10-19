namespace MTK.Modules.Communications.Application.FileAttachments.AddFileAttachment;

public sealed record AddFileAttachmentResponse(
    Guid Id,
    Guid ArticleId,
    string FileName,
    string MimeType,
    bool IsPublic);
