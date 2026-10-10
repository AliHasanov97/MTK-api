namespace MTK.Modules.Hr.Domain.Helpers;

/// <summary>
/// Helper class for DateTime and DateTimeOffset operations
/// </summary>
public static class DateTimeHelper
{
    /// <summary>
    /// Converts DateTimeOffset to UTC date-only value (time set to 00:00:00)
    /// This prevents timezone conversion issues when storing dates in the database
    /// </summary>
    /// <param name="dateTimeOffset">The DateTimeOffset value from client (may include timezone offset)</param>
    /// <returns>DateTimeOffset with only date component in UTC (offset 0)</returns>
    /// <example>
    /// Input: 2026-06-07T00:00:00+04:00 (Azerbaijan timezone)
    /// Output: 2026-06-07T00:00:00Z (UTC)
    /// </example>
    public static DateTimeOffset ToUtcDateOnly(DateTimeOffset dateTimeOffset)
    {
        return new DateTimeOffset(dateTimeOffset.DateTime.Date, TimeSpan.Zero);
    }

    /// <summary>
    /// Converts nullable DateTimeOffset to UTC date-only value
    /// </summary>
    /// <param name="dateTimeOffset">The nullable DateTimeOffset value</param>
    /// <returns>DateTimeOffset with only date component in UTC, or null if input is null</returns>
    public static DateTimeOffset? ToUtcDateOnly(DateTimeOffset? dateTimeOffset)
    {
        return dateTimeOffset.HasValue
            ? new DateTimeOffset(dateTimeOffset.Value.DateTime.Date, TimeSpan.Zero)
            : null;
    }
}