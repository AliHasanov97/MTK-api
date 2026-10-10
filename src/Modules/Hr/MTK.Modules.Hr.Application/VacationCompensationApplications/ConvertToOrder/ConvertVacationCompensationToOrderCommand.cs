using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.ConvertToOrder;

/// <summary>
/// VacationCompensationApplication-ı CompensationOrder-ə çevirir
/// </summary>
public sealed record ConvertVacationCompensationToOrderCommand(
    Guid CompensationApplicationId
) : ICommand<ConvertVacationCompensationToOrderResponse>;