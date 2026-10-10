using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.Timesheets.GetTimesheets;

public sealed class GetTimesheetsResponse
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int DaysInMonth { get; set; }
    public List<EmployeeTimesheetDto> Employees { get; set; } = new();
}

public sealed class EmployeeTimesheetDto
{
    public ResponseObjectWithName Employee { get; set; } = null!;
    public ResponseObjectWithName? Position { get; set; }
    public int RegisterNumber { get; set; }
    public List<TimesheetDayDto> Days { get; set; } = new();

    // Faktiki göstəricilər
    /// <summary>
    /// Aylıq faktiki iş günləri
    /// </summary>
    public int ActualWorkingDays { get; set; }

    /// <summary>
    /// Aylıq faktiki iş saatı
    /// </summary>
    public decimal ActualWorkedHours { get; set; }

    // Norma göstəriciləri
    /// <summary>
    /// Aylıq iş günləri norma (qrafikə görə)
    /// </summary>
    public int NormWorkingDays { get; set; }

    /// <summary>
    /// Aylıq iş saatı norma (qrafikə görə)
    /// </summary>
    public decimal NormWorkedHours { get; set; }

    // Məzuniyyət və digər günlər
    /// <summary>
    /// Əmək məzuniyyəti günləri (M)
    /// </summary>
    public int AnnualLeaveDays { get; set; }

    /// <summary>
    /// Sosial məzuniyyət günləri (Y)
    /// </summary>
    public int SocialLeaveDays { get; set; }

    /// <summary>
    /// Təhsil məzuniyyəti günləri (TM)
    /// </summary>
    public int EducationLeaveDays { get; set; }

    /// <summary>
    /// Ödənişsiz məzuniyyət günləri (ÖM)
    /// </summary>
    public int UnpaidLeaveDays { get; set; }

    /// <summary>
    /// Xəstəlik vərəqəsi günləri (X)
    /// </summary>
    public int SickLeaveDays { get; set; }

    /// <summary>
    /// Ezamiyyət günləri (E)
    /// </summary>
    public int BusinessTripDays { get; set; }

    /// <summary>
    /// Bayram/Hüzün günləri (B, H)
    /// </summary>
    public int HolidayDays { get; set; }

    /// <summary>
    /// Qeyri-iş günləri (Q)
    /// </summary>
    public int NonWorkingDays { get; set; }

    /// <summary>
    /// Üzrsüz işə gəlməmə günləri (İG)
    /// </summary>
    public int UnexcusedAbsenceDays { get; set; }
}

public sealed class TimesheetDayDto
{
    public DateOnly Date { get; set; }
    public int Day { get; set; }
    public string DayType { get; set; } = string.Empty;
    public string DayCode { get; set; } = string.Empty;
    public decimal? WorkedHours { get; set; }
}
