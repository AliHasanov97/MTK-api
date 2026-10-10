using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.Warnings;

public static class WarningErrors
{
    public static Error NotFound => new("Warning.NotFound", "Xəbərdarlıq tapılmadı.");
}
