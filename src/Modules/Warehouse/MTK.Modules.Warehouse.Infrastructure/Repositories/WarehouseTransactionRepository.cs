using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Data;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;
using MTK.Modules.Warehouse.Infrastructure.Database;

namespace MTK.Modules.Warehouse.Infrastructure.Repositories;

internal sealed class WarehouseTransactionRepository 
    : Repository<WarehouseTransaction, WarehouseDbContext>, IWarehouseTransactionRepository
{
    public WarehouseTransactionRepository(WarehouseDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<List<WarehouseTransaction>> GetByNomenclatureIdAsync(
        Guid nomenclatureId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.WarehouseTransactions
            .Where(t => t.NomenclatureId == nomenclatureId)
            .Include(t => t.Nomenclature)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WarehouseTransaction>> GetByTransactionTypeAsync(
        TransactionType transactionType,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.WarehouseTransactions
            .Where(t => t.TransactionType == transactionType)
            .Include(t => t.Nomenclature)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WarehouseTransaction>> GetByDateRangeAsync(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.WarehouseTransactions
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .Include(t => t.Nomenclature)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<WarehouseTransaction?> GetByReferenceAsync(
        string referenceType,
        Guid referenceId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.WarehouseTransactions
            .Include(t => t.Nomenclature)
            .FirstOrDefaultAsync(
                t => t.ReferenceType == referenceType && t.ReferenceId == referenceId,
                cancellationToken);
    }
}
