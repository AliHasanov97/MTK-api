using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationReturnApplications.ConvertToOrder;

/// <summary>
/// VacationReturnApplication-ı VacationReturnOrder-ə çevirir
/// </summary>
public sealed record ConvertVacationReturnToOrderCommand(
    Guid VacationReturnApplicationId
) : ICommand<ConvertVacationReturnToOrderResponse>;