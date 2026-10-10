using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.EducationLeaveOrders.GetEducationLeaveOrderById;

public sealed class GetEducationLeaveOrderByIdResponse
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateOnly? ReturnToWorkDate { get; set; }
    public string? Reason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
}
