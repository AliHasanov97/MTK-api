using MediatR;

namespace MTK.Common.Domain.Abstractions;

public abstract record DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOnUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; init; }

    public DateTimeOffset OccurredOnUtc { get; init; }
}
