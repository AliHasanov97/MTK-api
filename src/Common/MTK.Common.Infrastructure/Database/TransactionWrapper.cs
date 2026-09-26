using MTK.Common.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MTK.Common.Infrastructure.Database;

public class TransactionWrapper : ITransaction
{
    private readonly DbContext _context;
    private readonly List<Func<Task>> _afterCommitActions = new();
    private readonly IDbContextTransaction _transaction;

    private TransactionWrapper(DbContext context, IDbContextTransaction transaction)
    {
        _context = context;
        _transaction = transaction;
    }

    public static async Task<TransactionWrapper> BeginTransactionAsync(DbContext context,
        CancellationToken cancellationToken = default)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return new TransactionWrapper(context, transaction);
    }

    public static TransactionWrapper Wrap(DbContext context, IDbContextTransaction transaction)
        => new TransactionWrapper(context, transaction);

    internal IDbContextTransaction Inner => _transaction;

    public void Dispose()
    {
        _transaction?.Dispose();
        _afterCommitActions.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(CancellationToken.None);
    }

    public async Task DisposeAsync(CancellationToken cancellationToken)
    {
        await _transaction.DisposeAsync();
        _afterCommitActions.Clear();
    }

    public void AddPostCommitAction(Func<Task> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action), "Action cannot be null.");
        }

        _afterCommitActions.Add(action);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_afterCommitActions.Count > 0)
        {
            foreach (var action in _afterCommitActions)
            {
                await action.Invoke();
            }

            _afterCommitActions.Clear();
        }

        await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_afterCommitActions.Count > 0)
        {
            _afterCommitActions.Clear();
        }

        await _transaction.RollbackAsync(cancellationToken);
    }

    public Task CreateSavepointAsync(string name, CancellationToken cancellationToken = default)
        => _transaction.CreateSavepointAsync(name, cancellationToken);

    public Task RollbackToSavepointAsync(string name, CancellationToken cancellationToken = default)
        => _transaction.RollbackToSavepointAsync(name, cancellationToken);

    public Task ReleaseSavepointAsync(string name, CancellationToken cancellationToken = default)
        => _transaction.ReleaseSavepointAsync(name, cancellationToken);
}
