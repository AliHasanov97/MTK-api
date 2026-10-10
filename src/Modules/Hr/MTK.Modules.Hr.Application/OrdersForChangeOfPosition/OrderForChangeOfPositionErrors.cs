using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.OrdersForChangeOfPosition;

public static class OrderForChangeOfPositionErrors
{
    public static readonly Error NotFound = new Error(
        "OrderForChangeOfPosition.NotFound",
        "The order for change of position was not found");

    public static readonly Error ConvertFailed = new Error(
        "OrderForChangeOfPosition.ConvertFailed",
        "Vəzifə dəyişikliyi əmrinə çevrilməsində xəta baş verdi");
}
