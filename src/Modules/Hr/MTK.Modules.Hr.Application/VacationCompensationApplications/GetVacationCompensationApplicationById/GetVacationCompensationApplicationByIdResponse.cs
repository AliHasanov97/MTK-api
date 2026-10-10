using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.GetVacationCompensationApplicationById;

public sealed class GetVacationCompensationApplicationByIdResponse
{
    public Guid Id { get; set; }
    public int ApplicationNumber { get; set; }
    public string Status { get; set; } = null!;
    public ResponseObjectWithName Employee { get; set; } = null!;
    public int RequestedDays { get; set; }
    public DateOnly? WorkYearStart { get; set; }
    public DateOnly? WorkYearEnd { get; set; }
    public string? Notes { get; set; }
    public ResponseObjectWithName? CompensationOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
}