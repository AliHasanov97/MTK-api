using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.Warnings;

public static class WarningErrors
{
    public static Error NotFound => new("Warning.NotFound", "Xəbərdarlıq tapılmadı.");
    public static Error CompanyNotFound => new("Warning.CompanyNotFound", "Şirkət tapılmadı.");
    public static Error AddFailed => new("Warning.AddFailed", "Xəbərdarlıq əlavə edilərkən xəta baş verdi.");
}
