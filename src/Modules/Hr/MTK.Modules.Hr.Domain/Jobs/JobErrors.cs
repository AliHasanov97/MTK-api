using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.Jobs;

public static class JobErrors
{
    public static Error NotFound => new("Job.NotFound", "Vəzifə tapılmadı.");
    public static Error DepartmentJobNotFound => new("Job.NotFound", "Departament-vəzifə əlaqəsi tapılmadı.");
    public static Error DepartmentJobInUse => new("Job.InUse", "Vəzifə istifadədədir və silinə bilməz.");
}
