using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;

public static class EmploymentStatusChangeOrderErrors
{
    public static Error NotFound => new("EmploymentStatusChangeOrder.NotFound", "İş rejimi dəyişikliyi əmri tapılmadı.");
}
