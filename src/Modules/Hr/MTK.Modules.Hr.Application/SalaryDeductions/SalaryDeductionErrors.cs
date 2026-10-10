using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.SalaryDeductions;

public static class SalaryDeductionErrors
{
    public static readonly Error NotFound = new Error(
        "SalaryDeduction.NotFound",
        "Əmək haqqından tutma əmri tapılmadı");

    public static readonly Error CompanyNotFound = new Error(
        "SalaryDeduction.CompanyNotFound",
        "Şirkət tapılmadı");

    public static readonly Error AddFailed = new Error(
        "SalaryDeduction.AddFailed",
        "Əmək haqqından tutma əmri əlavə edilə bilmədi");

    public static readonly Error DeleteFailed = new Error(
        "SalaryDeduction.DeleteFailed",
        "Əmək haqqından tutma əmri silinə bilmədi");
}