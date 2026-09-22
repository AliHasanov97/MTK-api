namespace MTK.Common.Domain.Abstractions;

/// <summary>
/// Generic repository interface
/// </summary>
public interface IRepository<TEntity> where TEntity : Entity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
