using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications.ConvertToOrder;

/// <summary>
/// EducationLeaveApplication-ı EducationLeaveOrder-ə çevirir
/// </summary>
public sealed record ConvertEducationLeaveToOrderCommand(
    Guid EducationLeaveApplicationId
) : ICommand<ConvertEducationLeaveToOrderResponse>;
