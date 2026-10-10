using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.EmployeeWorkSchedules;

public static class EmployeeWorkScheduleErrors
{
    public static readonly Error NotFound = new Error(
        "EmployeeWorkSchedule.NotFound",
        "İşçinin iş qrafiki tapılmadı");
}
