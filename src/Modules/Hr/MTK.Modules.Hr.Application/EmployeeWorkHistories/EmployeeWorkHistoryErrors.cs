using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.EmployeeWorkHistories;

public static class EmployeeWorkHistoryErrors
{
    public static Error NotFound => new("EmployeeWorkHistoryNotFound", "İş tarixçəsi tapılmadı.");
    public static Error EmployeeNotFound => new("EmployeeWorkHistory.EmployeeNotFound", "İşçi tapılmadı.");
    public static Error CreateError => new("EmployeeWorkHistoryCreateError", "İş tarixçəsi yaradılarkən xəta baş verdi.");
    public static Error UpdateError => new("EmployeeWorkHistoryUpdateError", "İş tarixçəsi yenilənərkən xəta baş verdi.");
    public static Error DeleteError => new("EmployeeWorkHistoryDeleteError", "İş tarixçəsi silinərkən xəta baş verdi.");
    public static Error InvalidDateRange => new("InvalidDateRange", "Başlama tarixi bitmə tarixindən sonra ola bilməz.");
    public static Error AddFailed => new("EmployeeWorkHistory.AddFailed", "İş tarixçəsi əlavə edilərkən xəta baş verdi.");
    public static Error UpdateFailed => new("EmployeeWorkHistory.UpdateFailed", "İş təcrübəsi yenilənərkən xəta baş verdi.");
}
