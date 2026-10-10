using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.OrdersForChangeOfPosition.DeleteOrderForChangeOfPosition;

public sealed class DeleteOrderForChangeOfPositionCommand : ICommand
{
    public Guid Id { get; set; }
}
