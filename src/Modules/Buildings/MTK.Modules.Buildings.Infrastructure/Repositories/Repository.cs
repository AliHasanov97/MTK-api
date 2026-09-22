using Microsoft.EntityFrameworkCore;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

/// <summary>
/// Generic repository base class
/// </summary>
internal abstract class Repository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    protected readonly BuildingsDbContext DbContext;

    protected Repository(BuildingsDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual void Add(TEntity entity)
    {
        DbContext.Set<TEntity>().Add(entity);
    }

    public virtual void Update(TEntity entity)
    {
        DbContext.Set<TEntity>().Update(entity);
    }

    public virtual void Remove(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }
}
