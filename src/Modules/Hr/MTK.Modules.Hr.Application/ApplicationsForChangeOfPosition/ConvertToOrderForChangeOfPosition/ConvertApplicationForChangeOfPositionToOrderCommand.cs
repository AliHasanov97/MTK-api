using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.ConvertToOrderForChangeOfPosition;

public sealed class ConvertApplicationForChangeOfPositionToOrderCommand : ICommand<ConvertApplicationForChangeOfPositionToOrderResponse>
{
    public Guid ApplicationId { get; set; }
}
