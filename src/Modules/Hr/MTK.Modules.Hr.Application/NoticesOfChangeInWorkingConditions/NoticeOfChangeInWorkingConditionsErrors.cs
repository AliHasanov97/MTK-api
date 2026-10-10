using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions;

public static class NoticeOfChangeInWorkingConditionsErrors
{
    public static Error NotFound = new Error(
        "NoticeOfChangeInWorkingConditions.NotFound",
        "Göstərilən identifikatorla iş şəraitində dəyişiklik bildirişi tapılmadı");

    public static Error AddFailed = new Error(
        "NoticeOfChangeInWorkingConditions.AddFailed",
        "İş şəraitində dəyişiklik bildirişi əlavə edilərkən xəta baş verdi");

    public static Error UpdateFailed = new Error(
        "NoticeOfChangeInWorkingConditions.UpdateFailed",
        "İş şəraitində dəyişiklik bildirişi yenilənərkən xəta baş verdi");
}