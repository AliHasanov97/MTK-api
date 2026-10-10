using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.BonusOrders;

public static class BonusOrderErrors
{
    public static readonly Error NotFound = new Error(
        "BonusOrder.NotFound",
        "Mükafat əmri tapılmadı");

    public static readonly Error CompanyNotFound = new Error(
        "BonusOrder.CompanyNotFound",
        "Şirkət tapılmadı");

    public static readonly Error AddFailed = new Error(
        "BonusOrder.AddFailed",
        "Mükafat əmri əlavə edilə bilmədi");

    public static readonly Error DeleteFailed = new Error(
        "BonusOrder.DeleteFailed",
        "Mükafat əmri silinə bilmədi");
}
