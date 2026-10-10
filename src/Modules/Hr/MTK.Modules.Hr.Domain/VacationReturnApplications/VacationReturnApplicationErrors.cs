using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationReturnApplications;

public static class VacationReturnApplicationErrors
{
    public static readonly Error NotFound = new Error(
        "VacationReturnApplication.NotFound",
        "Məzuniyyətdən geri qayıtma ərizəsi tapılmadı");

    public static readonly Error AlreadyConvertedToOrder = new Error(
        "VacationReturnApplication.AlreadyConvertedToOrder",
        "Bu ərizə artıq əmrə çevrilmişdir");

    public static readonly Error CannotUpdateNonPendingApplication = new Error(
        "VacationReturnApplication.CannotUpdateNonPendingApplication",
        "Yalnız 'Gözləyir' statusunda olan ərizələr dəyişdirilə bilər");

    public static readonly Error CannotDeleteNonPendingApplication = new Error(
        "VacationReturnApplication.CannotDeleteNonPendingApplication",
        "Yalnız 'Gözləyir' statusunda olan ərizələr silinə bilər");
}
