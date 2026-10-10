using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.FileAttachments;

namespace MTK.Modules.Hr.Application.OrdersForChangeOfPosition.GetOrderForChangeOfPositionById;

public sealed class GetOrderForChangeOfPositionByIdResponse
{
    public Guid Id { get; init; }
    public int OrderNumber { get; init; }
    public Guid ApplicationForChangeOfPositionId { get; init; }
    public ResponseObjectWithName? Employee { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public ResponseObjectWithName? CreatedBy { get; init; }
}
