using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.VacationReturnOrders.GetVacationReturnOrderById;

public sealed class GetVacationReturnOrderByIdResponse
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public DateTimeOffset ReturnDate { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
}