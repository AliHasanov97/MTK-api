namespace MTK.Common.Domain.Abstractions;

public interface ITransaction : IDisposable, IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
    Task DisposeAsync(CancellationToken cancellationToken);
    void AddPostCommitAction(Func<Task> action);

    Task CreateSavepointAsync(string name, CancellationToken cancellationToken = default);
    Task RollbackToSavepointAsync(string name, CancellationToken cancellationToken = default);
    Task ReleaseSavepointAsync(string name, CancellationToken cancellationToken = default);
}
