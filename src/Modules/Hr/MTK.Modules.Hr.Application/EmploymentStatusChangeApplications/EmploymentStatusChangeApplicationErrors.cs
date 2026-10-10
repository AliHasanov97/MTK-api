using MTK.Common.Domain.Abstractions;
using DomainErrors = MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications.EmploymentStatusChangeApplicationErrors;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications;

public static class EmploymentStatusChangeApplicationErrors
{
    // Domain-dən gələn errorlar
    public static Error NotFound => DomainErrors.NotFound;
    public static Error SameEmploymentType => DomainErrors.SameEmploymentType;

    // Application-specific errorlar
    public static readonly Error EmployeeNotFound = new Error(
        "EmploymentStatusChangeApplication.EmployeeNotFound",
        "İşçi tapılmadı");

    public static readonly Error CompanyNotFound = new Error(
        "EmploymentStatusChangeApplication.CompanyNotFound",
        "Şirkət tapılmadı");

    public static readonly Error SupervisorNotFound = new Error(
        "EmploymentStatusChangeApplication.SupervisorNotFound",
        "Əmrin icrasına nəzarət edən işçi tapılmadı");

    public static readonly Error AlreadyConverted = new Error(
        "EmploymentStatusChangeApplication.AlreadyConverted",
        "Ərizə artıq əmrə çevrilmişdir");

    public static readonly Error CannotUpdateConvertedApplication = new Error(
        "EmploymentStatusChangeApplication.CannotUpdateConvertedApplication",
        "Əmrə çevrilmiş ərizəni yeniləmək olmaz");

    public static readonly Error AddFailed = new Error(
        "EmploymentStatusChangeApplication.AddFailed",
        "İş rejimi dəyişikliyi ərizəsi əlavə edilərkən xəta baş verdi");

    public static readonly Error UpdateFailed = new Error(
        "EmploymentStatusChangeApplication.UpdateFailed",
        "İş rejimi dəyişikliyi ərizəsi yenilənərkən xəta baş verdi");
}
