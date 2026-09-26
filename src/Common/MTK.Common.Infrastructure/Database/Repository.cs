using System.Linq.Expressions;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Domain.Queries;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MTK.Common.Infrastructure.Database;

public class Repository<T>: IRepository<T> where T : Entity
{
    protected readonly DbContext Context;
    protected DbSet<T> DbItem => Context.Set<T>();

    protected Repository(DbContext context)
    {
        Context = context;
    }

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await TransactionWrapper.BeginTransactionAsync(Context, cancellationToken);
        return transaction;
    }

    public Task<List<T>> ListFromIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        return DbItem
            .Where(i => ids.Contains(i.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken)
    {
        var entityType = DbItem.EntityType.ClrType;
    
        // Check if Name property exists using reflection
        var nameProperty = entityType.GetProperty("Name");
    
        // If Name property doesn't exist, return true (consider it unique)
        if (nameProperty == null)
        {
            return true;
        }
    
        // If Name property exists, check for uniqueness
        return !await DbItem
            .AnyAsync(i => EF.Property<string>(i, "Name") == name, cancellationToken);
    }

    public virtual async Task<T?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context
            .Set<T>()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }
    
    public virtual async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await Context
            .Set<T>()
            .ToListAsync(cancellationToken);
    }

    public async Task SyncAddAndDeleteAsync(List<T>? entitiesToAdd, List<Guid>? entitiesToDelete, ITransaction? transaction, CancellationToken cancellationToken = default)
    {
        if (entitiesToAdd == null && entitiesToDelete == null)
        {
            return;
        }

        if (entitiesToAdd != null)
        {
            foreach (var entity in entitiesToAdd)
            {
                Context.Set<T>().Add(entity);
            }
            await Context.SaveChangesAsync(cancellationToken);
        }

        if (entitiesToDelete != null && entitiesToDelete.Count > 0)
        {
            await DeleteRangeAsync(entitiesToDelete, transaction, cancellationToken);
        }
    }

    public async Task SyncAddAndDeleteAsync(List<T>? oldEntities, List<T>? newEntities, ITransaction? transaction, CancellationToken cancellationToken = default)
    {
        if (oldEntities == null && newEntities == null)
        {
            return;
        }

        var entitiesToAdd = newEntities?.Where(n => oldEntities == null || oldEntities.All(o => o.Id != n.Id)).ToList();
        if (entitiesToAdd != null && entitiesToAdd.Count > 0)
        {
            foreach (var entity in entitiesToAdd)
            {
                Context.Set<T>().Add(entity);
            }
            await Context.SaveChangesAsync(cancellationToken);
        }

        var entitiesToDelete = oldEntities?.Where(o => newEntities == null || newEntities.All(n => n.Id != o.Id)).ToList();
        if (entitiesToDelete != null && entitiesToDelete.Count > 0)
        {
            await DeleteRangeAsync(entitiesToDelete, transaction, cancellationToken);
        }
    }


    public virtual Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken = default)
    {
        return ListAsync(predicate, null, cancellationToken);
    }

    


    public virtual Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = Context.Set<T>();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query.ToListAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(Guid id, ITransaction? transaction, CancellationToken cancellationToken = default)
    {
        var entity = await Context.Set<T>().FindAsync(new object?[] { id }, cancellationToken);
        if (entity == null) return;
        await SoftDeleteWithProbeAsync(new[] { entity }, transaction, cancellationToken);
    }

    public virtual Task DeleteAsync(T entity, ITransaction? transaction, CancellationToken cancellationToken = default)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        return SoftDeleteWithProbeAsync(new[] { entity }, transaction, cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<Guid> ids, ITransaction? transaction, CancellationToken cancellationToken = default)
    {
        var entities = await Context.Set<T>().Where(i => ids.Contains(i.Id)).ToListAsync(cancellationToken);
        if (entities.Count == 0) return;
        await SoftDeleteWithProbeAsync(entities, transaction, cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, ITransaction? transaction, CancellationToken cancellationToken = default)
    {
        var list = entities as IReadOnlyCollection<T> ?? entities.ToList();
        if (list.Count == 0) return;
        await SoftDeleteWithProbeAsync(list, transaction, cancellationToken);
    }

    private void MarkAsDeleted(T entity)
    {
        entity.SetDeletedAt();
        Context.Entry(entity).State = EntityState.Modified;
    }

    private async Task SoftDeleteWithProbeAsync(IReadOnlyCollection<T> entities, ITransaction? transaction, CancellationToken cancellationToken)
    {
        var ownedTx = transaction == null
            ? await TransactionWrapper.BeginTransactionAsync(Context, cancellationToken)
            : null;
        var activeTx = transaction ?? ownedTx!;

        try
        {
            var savepoint = "del_probe_" + Guid.NewGuid().ToString("N");
            await activeTx.CreateSavepointAsync(savepoint, cancellationToken);

            try
            {
                Context.Set<T>().RemoveRange(entities);
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
            {
                await activeTx.RollbackToSavepointAsync(savepoint, cancellationToken);
                foreach (var entity in entities)
                {
                    Context.Entry(entity).State = EntityState.Unchanged;
                }
                throw;
            }

            await activeTx.RollbackToSavepointAsync(savepoint, cancellationToken);
            await activeTx.ReleaseSavepointAsync(savepoint, cancellationToken);

            foreach (var entity in entities)
            {
                Context.Attach(entity);
                MarkAsDeleted(entity);
            }
            await Context.SaveChangesAsync(cancellationToken);

            if (ownedTx != null)
            {
                await ownedTx.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            if (ownedTx != null)
            {
                await ownedTx.RollbackAsync(cancellationToken);
            }
            throw;
        }
        finally
        {
            if (ownedTx != null)
            {
                await ownedTx.DisposeAsync();
            }
        }
    }

    private static bool IsForeignKeyViolation(DbUpdateException ex)
        => ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.ForeignKeyViolation;

    public Task<List<T>> SearchAsync(CancellationToken cancellationToken = default)
    {
        return DbItem
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    public Task<List<T>> SearchAsync(List<QueryFilter>? filters, CancellationToken cancellationToken = default)
    {
        return SearchAsync(filters, null, null, cancellationToken);
    }

    public Task<List<T>> SearchAsync(SortCriteria? sortCriteria, CancellationToken cancellationToken = default)
    {
        return SearchAsync(null, sortCriteria, null, cancellationToken);
    }

    public Task<List<T>> SearchAsync(string? searchTerm, CancellationToken cancellationToken = default)
    {
        return SearchAsync(null, null, searchTerm, cancellationToken);
    }
    public Task<List<T>> SearchAsync(string? searchTerm, int? page, int? pageSize, CancellationToken cancellationToken = default)
    {
        return SearchAsync(null, null, searchTerm, page, pageSize, cancellationToken);
    }

    public Task<List<T>> SearchAsync(List<QueryFilter>? filters, SortCriteria? sortCriteria, CancellationToken cancellationToken = default)
    {
        return SearchAsync(filters, sortCriteria, null, cancellationToken);
    }
    
    public virtual async Task<List<T>> SearchAsync(List<QueryFilter>? filters, SortCriteria? sortCriteria, string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        return await SearchAsync(filters, sortCriteria, searchTerm, null, null, cancellationToken);
    }
    public virtual async Task<List<T>> SearchAsync(List<QueryFilter>? filters, SortCriteria? sortCriteria, string? searchTerm, int? page, int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFiltersAndSort(filters, sortCriteria);
        query = ApplyPages(query, page, pageSize);
        return await query
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(List<QueryFilter>? filters, SortCriteria? sortCriteria, string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFiltersAndSort(filters, sortCriteria);
        return await query
            .CountAsync(cancellationToken);
    }
    
    protected virtual IQueryable<T> ApplyPages(IQueryable<T> query, int? page, int? pageSize)
    {
        return ApplyPages<T>(query, page, pageSize);
    }
    protected virtual IQueryable<TItem> ApplyPages<TItem>(IQueryable<TItem> query, int? page, int? pageSize)
    {
        if (page is null || page < 0)
        {
            page = 0;
        }
        if (pageSize is null or <= 0)
        {
            pageSize = 100; // Default page size
        }
        
        query = query.Skip(page.Value * pageSize.Value).Take(pageSize.Value);
        return query;
    }
    
    protected virtual IQueryable<T> ApplyFiltersAndSort(List<QueryFilter>? filters, SortCriteria? sortCriteria)
    {
        return DbItem
            .ApplyFiltersAndSort(filters, sortCriteria);
    }

    public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        Context.Set<T>().Add(entity);
        return Context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        Context.Set<T>().AddRange(entities);
        await Context.SaveChangesAsync(cancellationToken);
    }
    public void AddRange(IEnumerable<T> entities)
    {
        Context.Set<T>().AddRange(entities);
    }

    public void Add(T entity)
    {
        Context.Set<T>().Add(entity);
    }
}