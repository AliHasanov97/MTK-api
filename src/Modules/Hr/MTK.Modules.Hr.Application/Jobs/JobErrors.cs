using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.Jobs;

public static class JobErrors
{
    public static Error NotFound => new("Job.NotFound", "Vəzifə tapılmadı.");
    public static Error UpdateFailed => new("Job.UpdateFailed", "Vəzifə yenilənərkən xəta baş verdi.");
    public static Error InUse => new("Job.InUse", "Vəzifə istifadədədir və silinə bilməz.");
}
