using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications.GetEducationLeaveApplicationById;

public sealed class GetEducationLeaveApplicationByIdResponse
{
    public Guid Id { get; set; }
    public int ApplicationNumber { get; set; }
    public string Status { get; set; } = null!;
    public ResponseObjectWithName Employee { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateOnly? ReturnToWorkDate { get; set; }
    public string? Reason { get; set; }
    public ResponseObjectWithName? Order { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
}
