using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationCompensationApplications;

public static class VacationCompensationErrors
{
    public static Error NotFound(Guid applicationId) => new(
        "VacationCompensationApplication.NotFound",
        $"Vacation compensation application '{applicationId}' tapılmadı");

    public static Error InvalidCompensationDays => new(
        "VacationCompensationApplication.InvalidCompensationDays",
        "Kompensasiya günləri 0-dan böyük olmalıdır");

}