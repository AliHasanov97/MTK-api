using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationApplications.ConvertToOrder;

/// <summary>
/// VacationApplication-ı VacationOrder-ə çevirir
/// Əmr ərizənin tarixləri və gün sayı ilə yaradılır
/// </summary>
public sealed record ConvertVacationToOrderCommand(
    Guid VacationApplicationId
) : ICommand<ConvertVacationToOrderResponse>;