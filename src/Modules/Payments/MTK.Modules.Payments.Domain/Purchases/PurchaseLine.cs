using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Purchases;

/// <summary>
/// Alış sənədinin bir sətri — alınan nomenklatura, miqdar və vahid qiymət.
///
/// Purchase aggregate-inin daxilindədir — yalnız Purchase metodları ilə
/// yaradılır/dəyişdirilir.
/// </summary>
public sealed class PurchaseLine : Entity
{
    private PurchaseLine() : base() { }

    public Guid PurchaseId { get; private set; }

    /// <summary>
    /// NomenclatureShadow-un Id-si — bu da Warehouse-dakı Nomenclature.Id ilə eynidir.
    /// Ayrı aggregate-dir → FK yox, yalnız Guid.
    /// </summary>
    public Guid NomenclatureId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    /// <summary>Sətrin cəmi (hesablanan, saxlanılmır).</summary>
    public decimal LineTotal => Quantity * UnitPrice;

    internal static PurchaseLine Create(Guid purchaseId, Guid nomenclatureId, decimal quantity, decimal unitPrice)
    {
        Validate(quantity, unitPrice);

        // Id qəsdən təyin edilmir: EF onu özü generasiya edir. Açar əvvəlcədən
        // təyin olunsa, EF bu sətri alışın kolleksiyasından kəşf edəndə onu mövcud
        // sayır və INSERT yerinə UPDATE yazır (ContractService-dəki eyni tələ).
        var line = new PurchaseLine
        {
            PurchaseId = purchaseId,
            NomenclatureId = nomenclatureId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };

        line.SetCreatedAt();
        return line;
    }

    internal void Update(decimal quantity, decimal unitPrice)
    {
        Validate(quantity, unitPrice);

        Quantity = quantity;
        UnitPrice = unitPrice;
        SetUpdatedAt();
    }

    private static void Validate(decimal quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Miqdar 0-dan böyük olmalıdır", nameof(quantity));
        if (unitPrice < 0)
            throw new ArgumentException("Vahid qiymət mənfi ola bilməz", nameof(unitPrice));
    }
}
