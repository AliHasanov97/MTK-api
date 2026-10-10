using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.GetUnpaidLeaveApplicationById;

public sealed class GetUnpaidLeaveApplicationByIdResponse
{
    public Guid Id { get; set; }
    public int ApplicationNumber { get; set; }
    public string Status { get; set; } = null!;
    public ResponseObjectWithName Employee { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateOnly? ReturnToWorkDate { get; set; }
    public string? Notes { get; set; }
    public ResponseObjectWithName? UnpaidLeaveOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
}
