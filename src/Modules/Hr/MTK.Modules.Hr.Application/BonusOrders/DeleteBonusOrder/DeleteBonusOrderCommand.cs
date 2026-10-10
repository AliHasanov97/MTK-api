using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.BonusOrders.DeleteBonusOrder;

public sealed class DeleteBonusOrderCommand : ICommand
{
    public Guid Id { get; set; }
}
