using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Data;
using MTK.Modules.Warehouse.Domain.WarehouseStock;
using MTK.Modules.Warehouse.Infrastructure.Database;

namespace MTK.Modules.Warehouse.Infrastructure.Repositories;

internal sealed class WarehouseStockRepository 
    : Repository<WarehouseStock, WarehouseDbContext>, IWarehouseStockRepository
{
    public WarehouseStockRepository(WarehouseDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<WarehouseStock?> GetByNomenclatureIdAsync(
        Guid nomenclatureId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.WarehouseStock
            .Include(s => s.Nomenclature)
            .FirstOrDefaultAsync(s => s.NomenclatureId == nomenclatureId, cancellationToken);
    }

    public async Task<List<WarehouseStock>> GetAllStockAsync(
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.WarehouseStock
            .Include(s => s.Nomenclature)
            .OrderBy(s => s.Nomenclature.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WarehouseStock>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.WarehouseStock
            .Include(s => s.Nomenclature)
            .Where(s => s.Nomenclature.MinStockLevel.HasValue &&
                       s.QuantityOnHand < s.Nomenclature.MinStockLevel.Value)
            .OrderBy(s => s.QuantityOnHand)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WarehouseStock>> GetOutOfStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.WarehouseStock
            .Include(s => s.Nomenclature)
            .Where(s => s.QuantityOnHand == 0)
            .OrderBy(s => s.Nomenclature.Name)
            .ToListAsync(cancellationToken);
    }
}
