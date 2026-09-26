namespace MTK.Common.Domain.Abstractions;

public abstract class Entity : IAuditable
{
    private readonly List<IDomainEvent> _domainEvents = new();

    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity()
    {
        Id = Guid.Empty;
    }

    public Guid Id { get; init; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.ToList();

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void SetCreatedAt()
    {
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void SetUpdatedAt()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetDeletedAt()
    {
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
