namespace MTK.Modules.Hr.Domain.Employees;

public sealed class WorkExperienceBreakdown
{
    public int Years { get; set; }
    public int Months { get; set; }
    public int Days { get; set; }
    public int TotalDays { get; set; }

    public static WorkExperienceBreakdown FromDays(int totalDays)
    {
        if (totalDays < 0)
            totalDays = 0;

        // 1 il = 365 gün
        // 1 ay = 30 gün (orta)
        var years = totalDays / 365;
        var remainingDays = totalDays % 365;

        var months = remainingDays / 30;
        var days = remainingDays % 30;

        return new WorkExperienceBreakdown
        {
            Years = years,
            Months = months,
            Days = days,
            TotalDays = totalDays
        };
    }

    public static WorkExperienceBreakdown FromDateRange(DateTimeOffset startDate, DateTimeOffset? endDate = null)
    {
        var end = endDate ?? DateTimeOffset.UtcNow;
        var totalDays = (int)(end - startDate).TotalDays;
        return FromDays(totalDays);
    }
}
