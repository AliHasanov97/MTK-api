using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.WarehouseStock.Events;

namespace MTK.Modules.Warehouse.Domain.WarehouseStock;

/// <summary>
/// Real-time anbar stoku (hər nomenklatura üçün cari balans)
/// </summary>
public sealed class WarehouseStock : Entity
{
    private WarehouseStock(
        Guid nomenclatureId,
        decimal quantityOnHand) : base(nomenclatureId)
    {
        NomenclatureId = nomenclatureId;
        QuantityOnHand = quantityOnHand;
        LastTransactionDate = null;
    }

    // Private constructor for EF Core
    private WarehouseStock() : base()
    {
    }

    /// <summary>
    /// Nomenklatura ID (həm PK, həm FK)
    /// </summary>
    public Guid NomenclatureId { get; private set; }

    /// <summary>
    /// Nomenklatura (navigation property)
    /// </summary>
    public Nomenclature Nomenclature { get; private set; } = null!;

    /// <summary>
    /// Əldə olan miqdar (cari stok)
    /// </summary>
    public decimal QuantityOnHand { get; private set; }

    /// <summary>
    /// Son əməliyyat tarixi
    /// </summary>
    public DateTimeOffset? LastTransactionDate { get; private set; }

    /// <summary>
    /// Yeni stok qeydi yaradır (ilk mal gəlişində)
    /// </summary>
    public static WarehouseStock Create(Guid nomenclatureId, decimal initialQuantity = 0)
    {
        var stock = new WarehouseStock(nomenclatureId, initialQuantity);
        stock.SetCreatedAt();
        return stock;
    }

    /// <summary>
    /// Stoku artırır (mal qəbulu)
    /// </summary>
    public void IncreaseStock(decimal quantity, DateTimeOffset? transactionDate = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Artırılacaq miqdar 0-dan böyük olmalıdır", nameof(quantity));

        QuantityOnHand += quantity;
        LastTransactionDate = transactionDate ?? DateTimeOffset.UtcNow;
        SetUpdatedAt();

        RaiseDomainEvent(new StockIncreasedDomainEvent(NomenclatureId, quantity, QuantityOnHand));
    }

    /// <summary>
    /// Stoku azaldır (mal çıxarışı)
    /// </summary>
    public void DecreaseStock(decimal quantity, DateTimeOffset? transactionDate = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Azaldılacaq miqdar 0-dan böyük olmalıdır", nameof(quantity));

        if (QuantityOnHand < quantity)
            throw new InvalidOperationException(
                $"Kifayət qədər stok yoxdur. Mövcud: {QuantityOnHand}, Tələb: {quantity}");

        QuantityOnHand -= quantity;
        LastTransactionDate = transactionDate ?? DateTimeOffset.UtcNow;
        SetUpdatedAt();

        RaiseDomainEvent(new StockDecreasedDomainEvent(NomenclatureId, quantity, QuantityOnHand));
    }

    /// <summary>
    /// Stoku sıfırlamaq (inventory adjustment)
    /// </summary>
    public void ResetStock(decimal newQuantity, DateTimeOffset? transactionDate = null)
    {
        if (newQuantity < 0)
            throw new ArgumentException("Yeni stok miqdarı mənfi ola bilməz", nameof(newQuantity));

        QuantityOnHand = newQuantity;
        LastTransactionDate = transactionDate ?? DateTimeOffset.UtcNow;
        SetUpdatedAt();
    }

    /// <summary>
    /// Kifayət qədər stok var?
    /// </summary>
    public bool HasSufficientStock(decimal requiredQuantity)
    {
        return QuantityOnHand >= requiredQuantity;
    }
}
