using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.OrdersForChangeOfPosition.GetOrderForChangeOfPositionById;

public sealed class GetOrderForChangeOfPositionByIdQuery : IQuery<GetOrderForChangeOfPositionByIdResponse>
{
    public Guid Id { get; set; }
}
