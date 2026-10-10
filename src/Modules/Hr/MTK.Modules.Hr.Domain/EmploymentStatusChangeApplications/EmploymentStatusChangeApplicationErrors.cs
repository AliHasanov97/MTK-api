using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;

public static class EmploymentStatusChangeApplicationErrors
{
    public static Error NotFound => new("EmploymentStatusChangeApplication.NotFound", "İş rejimi dəyişikliyi ərizəsi tapılmadı.");
    public static Error SameEmploymentType => new("EmploymentStatusChangeApplication.SameEmploymentType", "İşçinin cari iş rejimi ilə yeni iş rejimi eynidir.");
}
