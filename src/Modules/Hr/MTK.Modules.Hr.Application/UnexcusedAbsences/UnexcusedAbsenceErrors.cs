using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.UnexcusedAbsences;

public static class UnexcusedAbsenceErrors
{
    public static Error NotFound = new Error(
        "UnexcusedAbsence.NotFound",
        "Göstərilən identifikatorla üzrsüz işə gəlməmə tapılmadı");

    public static Error CompanyNotFound = new Error(
        "UnexcusedAbsence.CompanyNotFound",
        "Şirkət tapılmadı");

    public static Error AddFailed = new Error(
        "UnexcusedAbsence.AddFailed",
        "Üzrsüz işə gəlməmə əlavə edilərkən xəta baş verdi");
}
