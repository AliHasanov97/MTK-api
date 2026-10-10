using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders;

public static class WorkOnNonWorkdayOrderErrors
{
    public static readonly Error NotFound = new Error(
        "WorkOnNonWorkdayOrder.NotFound",
        "Qeyri-iş günündə işləmə əmri tapılmadı");

    public static readonly Error CompanyNotFound = new Error(
        "WorkOnNonWorkdayOrder.CompanyNotFound",
        "Şirkət tapılmadı");

    public static readonly Error AddFailed = new Error(
        "WorkOnNonWorkdayOrder.AddFailed",
        "Qeyri-iş günündə işləmə əmri əlavə edilə bilmədi");

    public static readonly Error DeleteFailed = new Error(
        "WorkOnNonWorkdayOrder.DeleteFailed",
        "Qeyri-iş günündə işləmə əmri silinə bilmədi");
}
