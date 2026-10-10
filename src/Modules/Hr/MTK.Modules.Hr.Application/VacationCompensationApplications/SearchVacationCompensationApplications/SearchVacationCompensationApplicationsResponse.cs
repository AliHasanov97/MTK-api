using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.SearchVacationCompensationApplications;

public sealed class SearchVacationCompensationApplicationsResponse
{
    public List<SearchVacationCompensationApplicationsResponseItem> Items { get; set; } = new();
    public int TotalCount { get; set; }
}

public sealed class SearchVacationCompensationApplicationsResponseItem
{
    public Guid Id { get; set; }
    public int ApplicationNumber { get; set; }
    public string Status { get; set; } = null!;
    public ResponseObjectWithName Employee { get; set; } = null!;
    public ResponseObjectWithName? Order { get; init; }
    public int RequestedDays { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}