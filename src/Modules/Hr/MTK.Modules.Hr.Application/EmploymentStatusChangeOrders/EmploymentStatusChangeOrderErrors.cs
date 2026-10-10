using MTK.Common.Domain.Abstractions;
using DomainErrors = MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders.EmploymentStatusChangeOrderErrors;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeOrders;

public static class EmploymentStatusChangeOrderErrors
{
    // Domain-dən gələn errorlar
    public static Error NotFound => DomainErrors.NotFound;

    // Application-specific errorlar
    public static readonly Error DeleteFailed = new Error(
        "EmploymentStatusChangeOrder.DeleteFailed",
        "İş rejimi dəyişikliyi əmri silinərkən xəta baş verdi");
}
