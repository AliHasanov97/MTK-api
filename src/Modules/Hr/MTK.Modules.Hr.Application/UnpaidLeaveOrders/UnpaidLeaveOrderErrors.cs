using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.UnpaidLeaveOrders;

public static class UnpaidLeaveOrderErrors
{
    public static Error NotFound => new("UnpaidLeaveOrderNotFound", "Ödənişsiz məzuniyyət əmri tapılmadı.");
}