using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.BonusOrders.GetBonusOrderById;

public sealed class GetBonusOrderByIdQuery : IQuery<GetBonusOrderByIdResponse>
{
    public Guid Id { get; set; }
}
