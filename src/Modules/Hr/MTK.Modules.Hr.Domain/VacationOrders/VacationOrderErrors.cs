using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationOrders;

public static class VacationOrderErrors
{
    public static readonly Error NotFound = new Error(
        "VacationOrder.NotFound",
        "Ödənişli məzuniyyət əmri tapılmadı");
}