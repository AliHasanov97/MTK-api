using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.ConvertToOrder;

public sealed class ConvertVacationCompensationToOrderResponse
{
    public Guid OrderId { get; set; }
    public int OrderNumber { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public int CompensatedDays { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}