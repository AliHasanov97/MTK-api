using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.CompensationOrders.GetCompensationOrderById;

public class GetCompensationOrderByIdResponse
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public int CompensatedDays { get; set; }
    public DateOnly? WorkYearStart { get; set; }
    public DateOnly? WorkYearEnd { get; set; }
    public string? Notes { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
}