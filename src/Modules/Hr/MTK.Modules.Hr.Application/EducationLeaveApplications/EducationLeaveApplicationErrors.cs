using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications;

public static class EducationLeaveApplicationErrors
{
    public static Error NotFound => new("EducationLeaveApplicationNotFound", "Təhsil məzuniyyəti müraciəti tapılmadı.");
    public static Error InvalidDates => new("EducationLeaveApplication.InvalidDates", "Bitmə tarixi başlama tarixindən əvvəl ola bilməz.");
    public static Error StartDateInPast => new("EducationLeaveApplication.StartDateInPast", "Başlama tarixi keçmişdə ola bilməz.");
    public static Error AlreadyConvertedToOrder => new("EducationLeaveApplicationAlreadyConverted", "Bu müraciət artıq əmrə çevrilmişdir.");
    public static Error CannotUpdateConvertedApplication => new("EducationLeaveApplication.CannotUpdate", "Əmrə çevrilmiş müraciəti yeniləmək olmaz.");
}
