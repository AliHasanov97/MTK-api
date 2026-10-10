namespace MTK.Modules.Hr.Domain.CalendarDays;

/// <summary>
/// Təqvim günü növü
/// </summary>
public enum CalendarDayType
{
    /// <summary>
    /// Bayram günü (qeyri-iş günü)
    /// </summary>
    Holiday = 1,

    /// <summary>
    /// İş günü edilmiş gün (məsələn: şənbə iş günü)
    /// </summary>
    WorkingDay = 2,

    /// <summary>
    /// Qeyri-iş günü edilmiş gün (bayram əvəzi)
    /// </summary>
    NonWorkingDay = 3
}
