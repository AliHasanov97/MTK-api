using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Application.CalendarDays.GetCalendarDaysByYear;

/// <summary>
/// İl üzrə təqvim günlərini əldə edir.
/// WorkingDays verilsə, yalnız həmin qrafikə aid günləri qaytarır.
/// </summary>
public sealed record GetCalendarDaysByYearQuery(
    int Year,
    WorkingDays? WorkingDays = null) : IQuery<GetCalendarDaysByYearResponse>;
