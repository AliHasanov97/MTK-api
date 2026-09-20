namespace MTK.Common.Application.Messaging;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
