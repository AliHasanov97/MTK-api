using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.ConvertToOrder;

/// <summary>
/// UnpaidLeaveApplication-ı UnpaidLeaveOrder-ə çevirir
/// </summary>
public sealed record ConvertUnpaidLeaveToOrderCommand(
    Guid UnpaidLeaveApplicationId
) : ICommand<ConvertUnpaidLeaveToOrderResponse>;
