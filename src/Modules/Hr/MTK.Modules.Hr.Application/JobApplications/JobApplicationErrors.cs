using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.JobApplications;

public static class JobApplicationErrors
{
    public static Error NotFound => new("JobApplicationNotFound", "İş müraciəti tapılmadı.");
    public static Error AlreadyConverted => new("JobApplicationAlreadyConverted", "Bu iş müraciəti artıq əmrə çevrilib.");
    public static Error JobNotFound => new("JobNotFound", "Vəzifə tapılmadı.");
    public static Error AddFailed => new("JobApplication.AddFailed", "İş ərizəsi əlavə edilərkən xəta baş verdi.");
    public static Error UpdateFailed => new("JobApplication.UpdateFailed", "İş müraciəti yenilənərkən xəta baş verdi.");
}
