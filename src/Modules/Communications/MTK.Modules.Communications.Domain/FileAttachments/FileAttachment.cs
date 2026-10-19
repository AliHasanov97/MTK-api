using MTK.Common.Domain.Abstractions;
using MTK.Modules.Communications.Domain.Articles;

namespace MTK.Modules.Communications.Domain.FileAttachments;

public class FileAttachment : SearchableEntity
{
    private FileAttachment() { }

    public FileAttachment(Guid id, string fileName, string mimeType, bool isPublic)
    {
        Id = id;
        FileName = fileName;
        MimeType = mimeType;
        IsPublic = isPublic;
    }

    public string FileName { get; set; } = null!;
    public string MimeType { get; set; } = null!;
    public bool IsPublic { get; set; }

    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;

    public static FileAttachment Create(
        Guid id,
        string fileName,
        string mimeType,
        bool isPublic,
        Guid articleId)
    {
        var attachment = new FileAttachment(id, fileName, mimeType, isPublic)
        {
            ArticleId = articleId
        };

        return attachment;
    }
}
