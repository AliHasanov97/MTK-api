using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Communications.Application.Abstractions.Data;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
