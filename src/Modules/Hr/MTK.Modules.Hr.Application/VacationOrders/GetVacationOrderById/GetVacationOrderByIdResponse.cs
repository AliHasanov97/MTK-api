using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.VacationOrders.GetVacationOrderById;

public sealed class GetVacationOrderByIdResponse
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public Guid VacationApplicationId { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int VacationDays { get; set; }
    public DateOnly? ReturnToWorkDate { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
}
