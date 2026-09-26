using System.Linq.Expressions;
using MTK.Common.Domain.Queries;

namespace MTK.Common.Domain.Abstractions;

/// <summary>
/// Generic repository interface
/// </summary>
public interface IRepository<T> where T : Entity
{
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task<List<T>> ListFromIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default);
    Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken);
    Task<T?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(T entity);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void AddRange(IEnumerable<T> entities);

    Task DeleteAsync(Guid id, ITransaction? transaction, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, ITransaction? transaction, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<Guid> ids, ITransaction? transaction, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<T> entities, ITransaction? transaction, CancellationToken cancellationToken = default);

    Task<List<T>> SearchAsync(CancellationToken cancellationToken = default);
    Task<List<T>> SearchAsync(List<QueryFilter>? filters, CancellationToken cancellationToken = default);
    Task<List<T>> SearchAsync(SortCriteria? sortCriteria, CancellationToken cancellationToken = default);
    Task<List<T>> SearchAsync(string? searchTerm, CancellationToken cancellationToken = default);
    Task<List<T>> SearchAsync(string? searchTerm, int? page, int? pageSize, CancellationToken cancellationToken = default);
    Task<List<T>> SearchAsync(List<QueryFilter>? filters, SortCriteria? sortCriteria, CancellationToken cancellationToken = default);
    Task<List<T>> SearchAsync(List<QueryFilter>? filters, SortCriteria? sortCriteria, string? searchTerm, CancellationToken cancellationToken = default);

    Task<List<T>> SearchAsync(List<QueryFilter>? filters, SortCriteria? sortCriteria, string? searchTerm,
        int? page, int? pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(List<QueryFilter>? filters, SortCriteria? sortCriteria, string? searchTerm, CancellationToken cancellationToken = default);

    Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy, CancellationToken cancellationToken = default);
    Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken = default);
    Task<List<T>> ListAsync(CancellationToken cancellationToken = default);

    Task SyncAddAndDeleteAsync(
        List<T>? entitiesToAdd,
        List<Guid>? entitiesToDelete,
        ITransaction? transaction,
        CancellationToken cancellationToken = default);
    Task SyncAddAndDeleteAsync(
        List<T>? oldEntities,
        List<T>? newEntities,
        ITransaction? transaction,
        CancellationToken cancellationToken = default);
}
