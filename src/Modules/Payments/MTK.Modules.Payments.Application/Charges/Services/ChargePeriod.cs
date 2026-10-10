using System.Globalization;
using System.Text.RegularExpressions;

namespace MTK.Modules.Payments.Application.Charges.Services;

/// <summary>"yyyy-MM" dövrü ilə iş: cari dövr (iş qurşağı ilə), düzgünlük yoxlaması, dövrün ilk günü.</summary>
public static partial class ChargePeriod
{
    [GeneratedRegex(@"^\d{4}-(0[1-9]|1[0-2])$")]
    private static partial Regex PeriodRegex();

    public static bool IsValid(string? period) => period is not null && PeriodRegex().IsMatch(period);

    /// <summary>Verilmiş UTC anının, göstərilən vaxt qurşağındakı təqvim ayına görə dövrü.</summary>
    public static string Current(DateTimeOffset utcNow, TimeZoneInfo timeZone) =>
        TimeZoneInfo.ConvertTime(utcNow, timeZone).ToString("yyyy-MM", CultureInfo.InvariantCulture);

    /// <summary>Dövrün ilk günü (UTC gecə yarısı). Format tanınmasa indiki an.</summary>
    public static DateTimeOffset FirstDay(string period) =>
        DateTimeOffset.TryParseExact(
            period + "-01",
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var parsed)
            ? parsed
            : DateTimeOffset.UtcNow;
}
