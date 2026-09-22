using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Infrastructure.Database;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly BuildingsDbContext _dbContext;

    public UnitOfWork(BuildingsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
