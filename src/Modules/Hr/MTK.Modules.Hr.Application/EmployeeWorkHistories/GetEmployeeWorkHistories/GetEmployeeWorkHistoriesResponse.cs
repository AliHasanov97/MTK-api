using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Application.EmployeeWorkHistories.GetEmployeeWorkHistories;

public sealed class GetEmployeeWorkHistoriesResponse
{
    public List<EmployeeWorkHistoryItem> data { get; set; } = new();
}

public sealed class EmployeeWorkHistoryItem
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string? Notes { get; set; }
    public WorkExperienceBreakdown Duration { get; set; } = null!;
}
