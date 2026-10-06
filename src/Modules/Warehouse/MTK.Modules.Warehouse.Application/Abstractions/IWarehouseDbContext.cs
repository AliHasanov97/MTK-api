using Microsoft.EntityFrameworkCore;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.WarehouseStock;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Application.Abstractions;

/// <summary>
/// Warehouse modulu üçün DbContext interface
/// </summary>
public interface IWarehouseDbContext
{
    DbSet<Nomenclature> Nomenclatures { get; }
    DbSet<WarehouseTransaction> WarehouseTransactions { get; }
    DbSet<WarehouseStock> WarehouseStock { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
