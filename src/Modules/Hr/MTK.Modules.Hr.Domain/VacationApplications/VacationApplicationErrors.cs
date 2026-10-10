using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationApplications;

public static class VacationApplicationErrors
{
    public static readonly Error NotFound = new Error(
        "VacationApplication.NotFound",
        "Məzuniyyət müraciəti tapılmadı");






    public static readonly Error AlreadyConvertedToOrder = new Error(
        "VacationApplication.AlreadyConvertedToOrder",
        "Bu müraciət artıq əmrə çevrilmişdir");

    public static readonly Error InvalidRequestedDays = new Error(
        "VacationApplication.InvalidRequestedDays",
        "RequestedDays 0-dan böyük olmalıdır");

    public static readonly Error BothEndDateAndRequestedDaysProvided = new Error(
        "VacationApplication.BothEndDateAndRequestedDaysProvided",
        "Yalnız EndDate və ya RequestedDays göndərin, hər ikisini yox");

    public static readonly Error NeitherEndDateNorRequestedDaysProvided = new Error(
        "VacationApplication.NeitherEndDateNorRequestedDaysProvided",
        "EndDate və ya RequestedDays göndərilməlidir");

    public static readonly Error CannotUpdateNonPendingApplication = new Error(
        "VacationApplication.CannotUpdateNonPendingApplication",
        "Yalnız 'Gözləyir' statusunda olan ərizələr dəyişdirilə bilər");
}
