using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Data;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Infrastructure.Database;

namespace MTK.Modules.Warehouse.Infrastructure.Repositories;

internal sealed class NomenclatureRepository : Repository<Nomenclature, WarehouseDbContext>, INomenclatureRepository
{
    public NomenclatureRepository(WarehouseDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Nomenclature?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await DbContext.Nomenclatures
            .FirstOrDefaultAsync(n => n.Code == code, cancellationToken);
    }

    public async Task<bool> IsCodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await DbContext.Nomenclatures
            .AnyAsync(n => n.Code == code, cancellationToken);
    }

    public async Task<bool> IsCodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Nomenclatures
            .AnyAsync(n => n.Code == code && n.Id != excludeId, cancellationToken);
    }

    public async Task<List<Nomenclature>> GetByCategoryAsync(
        NomenclatureCategory category,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Nomenclatures
            .Where(n => n.Category == category && n.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Nomenclature>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Nomenclatures
            .Where(n => n.IsActive && n.MinStockLevel.HasValue)
            .Include(n => n)
            .ToListAsync(cancellationToken);
    }
}
