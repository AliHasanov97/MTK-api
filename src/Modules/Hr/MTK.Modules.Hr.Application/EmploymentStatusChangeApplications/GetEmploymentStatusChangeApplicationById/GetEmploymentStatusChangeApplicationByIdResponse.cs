using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.GetEmploymentStatusChangeApplicationById;

public sealed class GetEmploymentStatusChangeApplicationByIdResponse
{
    public Guid Id { get; set; }
    public int ApplicationNumber { get; set; }
    public string Status { get; set; } = null!;
    public ResponseObjectWithName Employee { get; set; } = null!;
    public string CurrentEmploymentType { get; set; } = null!;
    public string NewEmploymentType { get; set; } = null!;
    public ResponseObjectWithName OrderExecutionSupervisor { get; set; } = null!;
    public ResponseObjectWithName? EmploymentStatusChangeOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
}
