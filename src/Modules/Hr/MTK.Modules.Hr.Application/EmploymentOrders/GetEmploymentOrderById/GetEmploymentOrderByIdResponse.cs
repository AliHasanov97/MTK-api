using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.EmploymentOrders.GetEmploymentOrderById;

public class GetEmploymentOrderByIdResponse
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public ResponseObjectWithName JobApplication { get; set; } = null!;
    public ResponseObjectWithName Employee { get; set; } = null!;
    public ResponseObjectWithName Job { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public ResponseObjectWithName? LaborCodeCase { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
