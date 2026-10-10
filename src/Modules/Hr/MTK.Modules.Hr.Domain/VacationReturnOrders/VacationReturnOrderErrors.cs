using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationReturnOrders;

public static class VacationReturnOrderErrors
{
    public static readonly Error NotFound = new Error(
        "VacationReturnOrder.NotFound",
        "Məzuniyyətdən geri qayıtma əmri tapılmadı");
}