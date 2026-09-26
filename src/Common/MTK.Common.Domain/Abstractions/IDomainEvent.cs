using MediatR;

namespace MTK.Common.Domain.Abstractions;

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    DateTimeOffset OccurredOnUtc { get; }
}
