namespace MTK.Modules.Warehouse.Domain.WarehouseTransactions;

/// <summary>
/// Anbar əməliyyatı tipi
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Daxilolma (mal qəbulu)
    /// </summary>
    Receipt = 1,

    /// <summary>
    /// Çıxarış (mal verilməsi)
    /// </summary>
    Issue = 2
}
