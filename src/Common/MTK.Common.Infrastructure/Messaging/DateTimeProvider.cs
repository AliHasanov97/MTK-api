using MTK.Common.Application.Messaging;

namespace MTK.Common.Infrastructure.Messaging;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
