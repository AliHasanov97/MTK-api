using MTK.Common.Application.Messaging;

namespace MTK.DataSeeder.Infrastructure;

/// <summary>
/// Fake DateTimeProvider for CLI/Seeder - real UTC time qaytarır
/// </summary>
internal sealed class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}