using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.Nomenclatures;

/// <summary>
/// Nomenklatura repository interface
/// </summary>
public interface INomenclatureRepository : IRepository<Nomenclature>
{
    /// <summary>
    /// Kod üzrə nomenklatura tapır
    /// </summary>
    Task<Nomenclature?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kod mövcuddur?
    /// </summary>
    Task<bool> IsCodeExistsAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kod mövcuddur (özündən başqa)?
    /// </summary>
    Task<bool> IsCodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kateqoriya üzrə aktiv nomenklaturalar
    /// </summary>
    Task<List<Nomenclature>> GetByCategoryAsync(
        NomenclatureCategory category,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Minimum ehtiyat səviyyəsindən aşağı olanları tapır (Stok baxılacaq)
    /// </summary>
    Task<List<Nomenclature>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
}
