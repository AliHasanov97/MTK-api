using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications;

public static class UnpaidLeaveApplicationErrors
{
    public static Error NotFound => new("UnpaidLeaveApplicationNotFound", "Ödənişsiz məzuniyyət müraciəti tapılmadı.");
    public static Error InvalidDates => new("UnpaidLeaveApplication.InvalidDates", "Bitmə tarixi başlama tarixindən əvvəl ola bilməz.");
    public static Error StartDateInPast => new("UnpaidLeaveApplication.StartDateInPast", "Başlama tarixi keçmişdə ola bilməz.");
    public static Error AlreadyConvertedToOrder => new("UnpaidLeaveApplicationAlreadyConverted", "Bu müraciət artıq əmrə çevrilmişdir.");
    public static Error CannotUpdateConvertedApplication => new("UnpaidLeaveApplication.CannotUpdate", "Əmrə çevrilmiş müraciəti yeniləmək olmaz.");
}