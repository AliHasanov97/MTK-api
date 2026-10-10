using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Domain.FileAttachments;

namespace MTK.Modules.Hr.Application.BonusOrders.GetBonusOrderById;

public sealed class GetBonusOrderByIdResponse
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public ResponseObjectWithName OrderExecutionSupervisor { get; set; } = null!;
    public int BonusQuantity { get; set; }
    public int SalaryMonth { get; set; }
    public int SalaryYear { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
}

