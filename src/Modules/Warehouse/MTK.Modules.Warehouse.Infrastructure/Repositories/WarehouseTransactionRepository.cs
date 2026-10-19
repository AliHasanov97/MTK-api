using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;
using MTK.Modules.Warehouse.Infrastructure.Database;

namespace MTK.Modules.Warehouse.Infrastructure.Repositories;

internal sealed class WarehouseTransactionRepository
    : Repository<WarehouseTransaction>, IWarehouseTransactionRepository
{
    private WarehouseDbContext WarehouseContext => (WarehouseDbContext)Context;

    public WarehouseTransactionRepository(WarehouseDbContext dbContext)
        : base(dbContext)
    {
    }

    public override Task<List<WarehouseTransaction>> ListAsync(CancellationToken cancellationToken = default)
    {
        return WarehouseContext.WarehouseTransactions
            .Include(t => t.Nomenclature)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WarehouseTransaction>> GetByNomenclatureIdAsync(
        Guid nomenclatureId,
        CancellationToken cancellationToken = default)
    {
        return await WarehouseContext.WarehouseTransactions
            .Where(t => t.NomenclatureId == nomenclatureId)
            .Include(t => t.Nomenclature)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WarehouseTransaction>> GetByTransactionTypeAsync(
        TransactionType transactionType,
        CancellationToken cancellationToken = default)
    {
        return await WarehouseContext.WarehouseTransactions
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
        return await WarehouseContext.WarehouseTransactions
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .Include(t => t.Nomenclature)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, decimal>> GetLatestUnitPricesAsync(
        IReadOnlyCollection<Guid> nomenclatureIds,
        CancellationToken cancellationToken = default)
    {
        if (nomenclatureIds.Count == 0)
        {
            return new Dictionary<Guid, decimal>();
        }

        var ids = nomenclatureIds.ToList();
        var rows = await WarehouseContext.WarehouseTransactions
            .AsNoTracking()
            .Where(t => ids.Contains(t.NomenclatureId) && t.UnitPrice != null)
            .Select(t => new { t.NomenclatureId, t.UnitPrice, t.TransactionDate })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(t => t.NomenclatureId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(t => t.TransactionDate).First().UnitPrice!.Value);
    }

    public async Task<WarehouseTransaction?> GetByReferenceAsync(
        string referenceType,
        Guid referenceId,
        CancellationToken cancellationToken = default)
    {
        return await WarehouseContext.WarehouseTransactions
            .Include(t => t.Nomenclature)
            .FirstOrDefaultAsync(
                t => t.ReferenceType == referenceType && t.ReferenceId == referenceId,
                cancellationToken);
    }

    public async Task<bool> ExistsByReferenceAsync(
        string referenceType,
        Guid referenceId,
        CancellationToken cancellationToken = default)
    {
        return await WarehouseContext.WarehouseTransactions
            .AnyAsync(
                t => t.ReferenceType == referenceType && t.ReferenceId == referenceId,
                cancellationToken);
    }
}
