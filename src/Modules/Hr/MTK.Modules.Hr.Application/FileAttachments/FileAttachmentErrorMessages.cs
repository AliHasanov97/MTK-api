using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.FileAttachments;

public static class FileAttachmentErrorMessages
{
    public static Error NotFound => new(
        "FileAttachmentNotFound",
        "Fayl əlavəsi tapılmadı."
    );

    public static Error InvalidRequest => new(
        "FileAttachmentInvalidRequest",
        "JobApplicationId təqdim edilməlidir."
    );

    public static Error CreateError => new(
        "FILE_ATTACHMENT_CREATE_FAILED",
        "Fayl əlavəsi yaradılarkən xəta baş verdi."
    );

    public static Error DeleteError => new(
        "FILE_ATTACHMENT_DELETE_FAILED",
        "Fayl əlavəsi silinərkən xəta baş verdi."
    );

    public static Error UnexpectedError => new(
        "UnexpectedError",
        "Gözlənilməz xəta baş verdi."
    );

    public static Error NoFilesProvided => new(
        "NoFilesProvided",
        "Heç bir fayl təqdim edilməyib."
    );
}
