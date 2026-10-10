using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.Employees;

public static class EmployeeErrors
{
    public static Error NotFound => new("EmployeeNotFound", "İşçi tapılmadı.");
    public static Error JobNotFound => new("Employee.JobNotFound", "Vəzifə tapılmadı.");
    public static Error CannotDeleteHiredViaAcceptanceOrder
        => new("Employee.CannotDeleteHiredViaAcceptanceOrder",
            "Qəbul əmri ilə qəbul edilmiş işçini silmək mümkün deyil.");
    public static Error AddFailed => new("Employee.AddFailed", "İşçi əlavə edilərkən xəta baş verdi.");
    public static Error UpdateFailed => new("Employee.UpdateFailed", "İşçi yenilənərkən xəta baş verdi.");
}
