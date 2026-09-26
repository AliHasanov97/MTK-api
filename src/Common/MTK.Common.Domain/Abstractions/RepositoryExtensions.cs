namespace MTK.Common.Domain.Abstractions;

/// <summary>
/// Extension methods for IRepository to provide backward compatibility
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Alias for GetByIdDefaultAsync for backward compatibility
    /// </summary>
    public static Task<T?> GetByIdAsync<T>(this IRepository<T> repository, Guid id, CancellationToken cancellationToken = default)
        where T : Entity
    {
        return repository.GetByIdDefaultAsync(id, cancellationToken);
    }

    /// <summary>
    /// Alias for ListAsync for backward compatibility
    /// </summary>
    public static Task<List<T>> GetAllAsync<T>(this IRepository<T> repository, CancellationToken cancellationToken = default)
        where T : Entity
    {
        return repository.ListAsync(cancellationToken);
    }
}
