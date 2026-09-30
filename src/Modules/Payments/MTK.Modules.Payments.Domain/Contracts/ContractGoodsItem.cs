using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Contracts;

/// <summary>
/// Müqavilə üzrə mal sətiri (məs. "Sement M500", vahid "torba").
///
/// Xidmətdən (<see cref="ContractService"/>) fərqi budur: cədvəl üzrə təkrarlanan
/// borc yaratmır. Mal sifarişində miqdar əvvəlcədən bilinmir — borc yalnız konkret
/// tədarük (qaimə) üzrə, göndərilən miqdara görə yaranır.
/// </summary>
public sealed class ContractGoodsItem : Entity
{
    private ContractGoodsItem() : base() { }

    public Guid ContractId { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    /// <summary>Ölçü vahidi — "ədəd", "torba", "kq", "m²" (nomenklatura modulu gələnə qədər sərbəst mətn).</summary>
    public string Unit { get; private set; } = string.Empty;

    /// <summary>Müqavilə ilə razılaşdırılmış vahid qiymət.</summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>Gözlənilən ümumi miqdar — yalnız istinad üçündür, borc hesabına təsir etmir.</summary>
    public decimal? AgreedQuantity { get; private set; }

    /// <summary>Ödəniş müddəti (gün) — yaranan borcun son ödəniş tarixi bu qədər sonraya düşür.</summary>
    public int? PaymentTermDays { get; private set; }

    public bool IsActive { get; private set; }

    internal static ContractGoodsItem Create(
        Guid contractId,
        string name,
        string unit,
        decimal unitPrice,
        decimal? agreedQuantity = null,
        int? paymentTermDays = null,
        string? description = null)
    {
        Validate(name, unit, unitPrice, agreedQuantity, paymentTermDays);

        // Id qəsdən təyin edilmir: EF onu özü generasiya edir. Açar əvvəlcədən
        // təyin olunsa, EF bu sətri müqavilənin kolleksiyasından kəşf edəndə onu
        // mövcud sayır və INSERT yerinə UPDATE yazır (DbUpdateConcurrencyException).
        var item = new ContractGoodsItem
        {
            ContractId = contractId,
            Name = name.Trim(),
            Unit = unit.Trim(),
            UnitPrice = unitPrice,
            AgreedQuantity = agreedQuantity,
            PaymentTermDays = paymentTermDays,
            Description = description,
            IsActive = true
        };

        item.SetCreatedAt();
        return item;
    }

    internal void Update(
        string name,
        string unit,
        decimal unitPrice,
        decimal? agreedQuantity,
        int? paymentTermDays,
        string? description)
    {
        Validate(name, unit, unitPrice, agreedQuantity, paymentTermDays);

        Name = name.Trim();
        Unit = unit.Trim();
        UnitPrice = unitPrice;
        AgreedQuantity = agreedQuantity;
        PaymentTermDays = paymentTermDays;
        Description = description;
        SetUpdatedAt();
    }

    internal void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    internal void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    private static void Validate(
        string name,
        string unit,
        decimal unitPrice,
        decimal? agreedQuantity,
        int? paymentTermDays)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Malın adı mütləqdir", nameof(name));
        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Ölçü vahidi mütləqdir", nameof(unit));
        if (unitPrice < 0)
            throw new ArgumentException("Vahid qiymət mənfi ola bilməz", nameof(unitPrice));
        if (agreedQuantity is <= 0)
            throw new ArgumentException("Gözlənilən miqdar müsbət olmalıdır", nameof(agreedQuantity));
        if (paymentTermDays is < 0)
            throw new ArgumentException("Ödəniş müddəti mənfi ola bilməz", nameof(paymentTermDays));
    }
}
