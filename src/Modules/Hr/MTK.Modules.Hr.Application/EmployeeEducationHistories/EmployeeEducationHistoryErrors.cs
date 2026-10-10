using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.EmployeeEducationHistories;

public static class EmployeeEducationHistoryErrors
{
    public static Error NotFound => new("EmployeeEducationHistory.NotFound", "Təhsil tarixi tapılmadı.");
    public static Error EmployeeNotFound => new("EmployeeEducationHistory.EmployeeNotFound", "İşçi tapılmadı.");
    public static Error EducationalInstitutionNotFound => new("EmployeeEducationHistory.EducationalInstitutionNotFound", "Təhsil müəssisəsi tapılmadı.");
    public static Error AddFailed => new("EmployeeEducationHistory.AddFailed", "Təhsil tarixçəsi əlavə edilərkən xəta baş verdi.");
    public static Error UpdateFailed => new("EmployeeEducationHistory.UpdateFailed", "Təhsil tarixçəsi yenilənərkən xəta baş verdi.");
}
