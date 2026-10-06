using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.WarehouseStock;

/// <summary>
/// Anbar stoku repository interface
/// </summary>
public interface IWarehouseStockRepository : IRepository<WarehouseStock>
{
    /// <summary>
    /// Nomenklatura ID üzrə stok tapır
    /// </summary>
    Task<WarehouseStock?> GetByNomenclatureIdAsync(
        Guid nomenclatureId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bütün stokları tapır (pagination)
    /// </summary>
    Task<List<WarehouseStock>> GetAllStockAsync(
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Minimum ehtiyat səviyyəsindən aşağı olanları tapır
    /// </summary>
    Task<List<WarehouseStock>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sıfır stoklu nomenklaturalar
    /// </summary>
    Task<List<WarehouseStock>> GetOutOfStockItemsAsync(CancellationToken cancellationToken = default);
}
