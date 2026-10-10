using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.EducationalInstitutions;

public static class EducationalInstitutionErrors
{
    public static Error NotFound => new("EducationalInstitution.NotFound", "Təhsil ocağı tapılmadı.");
    public static Error InUse => new("EducationalInstitution.InUse", "Təhsil ocağı istifadədədir və silinə bilməz.");
}
