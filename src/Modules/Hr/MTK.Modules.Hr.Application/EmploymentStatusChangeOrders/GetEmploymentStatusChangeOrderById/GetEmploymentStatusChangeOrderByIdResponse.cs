using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.GetEmploymentStatusChangeOrderById;

public sealed class GetEmploymentStatusChangeOrderByIdResponse
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public string CurrentEmploymentType { get; set; } = null!;
    public string NewEmploymentType { get; set; } = null!;
    public ResponseObjectWithName OrderExecutionSupervisor { get; set; } = null!;
    public ResponseObjectWithName Application { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
}
