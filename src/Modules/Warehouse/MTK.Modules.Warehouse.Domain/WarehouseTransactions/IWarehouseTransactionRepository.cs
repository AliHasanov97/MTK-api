using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.WarehouseTransactions;

/// <summary>
/// Anbar əməliyyatı repository interface
/// </summary>
public interface IWarehouseTransactionRepository : IRepository<WarehouseTransaction>
{
    /// <summary>
    /// Nomenklatura üzrə əməliyyatları tapır
    /// </summary>
    Task<List<WarehouseTransaction>> GetByNomenclatureIdAsync(
        Guid nomenclatureId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Əməliyyat tipi üzrə tapır
    /// </summary>
    Task<List<WarehouseTransaction>> GetByTransactionTypeAsync(
        TransactionType transactionType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Tarix aralığında əməliyyatları tapır
    /// </summary>
    Task<List<WarehouseTransaction>> GetByDateRangeAsync(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// İstinad üzrə əməliyyat tapır (məs: Accounting Purchase ID)
    /// </summary>
    Task<WarehouseTransaction?> GetByReferenceAsync(
        string referenceType,
        Guid referenceId,
        CancellationToken cancellationToken = default);
}
