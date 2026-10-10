using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationCompensationApplications;

public static class VacationCompensationApplicationErrors
{
    public static readonly Error NotFound = new Error(
        "VacationCompensationApplication.NotFound",
        "Məzuniyyət kompensasiyası müraciəti tapılmadı");

    public static readonly Error InvalidRequestedDays = new Error(
        "VacationCompensationApplication.InvalidRequestedDays",
        "Kompensasiya günləri 0-dan böyük olmalıdır");


    public static readonly Error AlreadyConvertedToOrder = new Error(
        "VacationCompensationApplication.AlreadyConvertedToOrder",
        "Bu müraciət artıq əmrə çevrilmişdir");
}
