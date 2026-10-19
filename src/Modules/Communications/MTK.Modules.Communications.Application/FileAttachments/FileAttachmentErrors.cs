using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Communications.Application.FileAttachments;

public static class FileAttachmentErrors
{
    public static Error NotFound => new("FileAttachment.NotFound", "Fayl tapılmadı.");
    public static Error ArticleNotFound => new("FileAttachment.ArticleNotFound", "Fayl əlavə ediləcək xəbər tapılmadı.");
    public static Error FileRequired => new("FileAttachment.FileRequired", "Fayl tələb olunur.");
    public static Error TooLarge => new("FileAttachment.TooLarge", "Fayl çox böyükdür (maksimum 20 MB).");
    public static Error CreateFailed => new("FileAttachment.CreateFailed", "Fayl yüklənərkən xəta baş verdi.");
    public static Error DeleteFailed => new("FileAttachment.DeleteFailed", "Fayl silinərkən xəta baş verdi.");
}
