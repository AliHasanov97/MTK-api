using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.EmploymentOrders;

public static class EmploymentOrderErrors
{
    public static Error NotFound => new("EmploymentOrderNotFound", "İş əmri tapılmadı.");
    public static Error JobApplicationNotFound => new("JobApplicationNotFound", "İş müraciəti tapılmadı.");
    public static Error LaborCodeCaseNotFound => new("EmploymentOrder.LaborCodeCaseNotFound", "Əmək məcəlləsi maddəsi tapılmadı.");
    public static Error UpdateFailed => new("EmploymentOrder.UpdateFailed", "İşə qəbul əmri yenilənərkən xəta baş verdi.");
    public static Error ConvertFailed => new("EmploymentOrder.ConvertFailed", "İşə qəbul əmrinə çevrilməsində xəta baş verdi.");
}
