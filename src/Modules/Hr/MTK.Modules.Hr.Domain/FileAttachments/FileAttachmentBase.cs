using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.FileAttachments;

public abstract class FileAttachmentBase : SearchableEntity
{
    public string FileName { get; set; } = null!;
    public string MimeType { get; set; } = null!;
    public bool IsPublic { get; set; }
}
