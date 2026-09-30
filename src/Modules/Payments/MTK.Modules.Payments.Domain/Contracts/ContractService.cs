using System.Globalization;
using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Contracts;

/// <summary>
/// Müqavilə üzrə göstərilən xidmət (məs. "Liftə aylıq texniki xidmət",
/// "Liftin illik sığortası").
///
/// Contract aggregate-inin daxilindədir — yalnız Contract metodları ilə
/// yaradılır/dəyişdirilir, çünki təkbaşına mənası yoxdur.
///
/// Dövr məlumatı burada saxlanılır ki, borc generasiyası müqavilədən asılı
/// olmadan xidmət səviyyəsində işləyə bilsin: aktiv xidmətin
/// <see cref="UnitPrice"/> × <see cref="Quantity"/> məbləği <see cref="BillingPeriod"/>
/// dövrü üzrə borc kimi yazılır.
/// </summary>
public sealed class ContractService : Entity
{
    private ContractService() : base() { }

    public Guid ContractId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    /// <summary>Ölçü vahidi — "ay" (texniki xidmət), "illik", "ədəd".</summary>
    public string Unit { get; private set; } = string.Empty;

    /// <summary>Vahid qiymət (məs. bir lift üçün aylıq 50 AZN).</summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>Miqdar (məs. 3 lift). Default 1.</summary>
    public decimal Quantity { get; private set; }

    public BillingPeriod BillingPeriod { get; private set; }

    /// <summary>Null olduqda müqavilənin başlanğıc tarixi tətbiq olunur.</summary>
    public DateTimeOffset? ServiceStartDate { get; private set; }

    /// <summary>Null olduqda müqavilənin bitmə tarixi tətbiq olunur.</summary>
    public DateTimeOffset? ServiceEndDate { get; private set; }

    /// <summary>Ödəniş müddəti (gün) — yaranan borcun son ödəniş tarixi bu qədər sonraya düşür.</summary>
    public int? PaymentTermDays { get; private set; }

    /// <summary>Tək xidməti dayandırmaq üçün (müqavilə aktiv qalır).</summary>
    public bool IsActive { get; private set; }

    /// <summary>Dövr üzrə məbləğ: UnitPrice × Quantity.</summary>
    public decimal PeriodAmount => UnitPrice * Quantity;

    /// <summary>Bu xidmət avtomatik borc yaradırmı (birdəfəlik xidmətlər istisna).</summary>
    public bool AutoCharge => BillingPeriod != BillingPeriod.OneTime;

    internal static ContractService Create(
        Guid contractId,
        string name,
        decimal unitPrice,
        BillingPeriod billingPeriod,
        decimal quantity = 1,
        string unit = "ay",
        string? description = null,
        DateTimeOffset? serviceStartDate = null,
        DateTimeOffset? serviceEndDate = null,
        int? paymentTermDays = null)
    {
        Validate(name, unit, unitPrice, quantity, paymentTermDays, serviceStartDate, serviceEndDate);

        // Id qəsdən təyin edilmir: EF onu özü generasiya edir. Açar əvvəlcədən
        // təyin olunsa, EF bu sətri müqavilənin kolleksiyasından kəşf edəndə onu
        // mövcud sayır və INSERT yerinə UPDATE yazır (DbUpdateConcurrencyException).
        var service = new ContractService
        {
            ContractId = contractId,
            Name = name.Trim(),
            Description = description,
            Unit = unit.Trim(),
            UnitPrice = unitPrice,
            Quantity = quantity,
            BillingPeriod = billingPeriod,
            ServiceStartDate = serviceStartDate,
            ServiceEndDate = serviceEndDate,
            PaymentTermDays = paymentTermDays,
            IsActive = true
        };

        service.SetCreatedAt();
        return service;
    }

    internal void Update(
        string name,
        decimal unitPrice,
        BillingPeriod billingPeriod,
        decimal quantity,
        string unit,
        string? description,
        DateTimeOffset? serviceStartDate,
        DateTimeOffset? serviceEndDate,
        int? paymentTermDays)
    {
        Validate(name, unit, unitPrice, quantity, paymentTermDays, serviceStartDate, serviceEndDate);

        Name = name.Trim();
        Description = description;
        Unit = unit.Trim();
        UnitPrice = unitPrice;
        Quantity = quantity;
        BillingPeriod = billingPeriod;
        ServiceStartDate = serviceStartDate;
        ServiceEndDate = serviceEndDate;
        PaymentTermDays = paymentTermDays;
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

    /// <summary>
    /// Verilmiş dövr ("2026-09") üçün bu xidmətdən borc yaranmalıdırmı?
    /// Generasiya job-u bu qaydaya əsaslanır.
    /// </summary>
    public bool IsBillableFor(DateTimeOffset contractStart, DateTimeOffset contractEnd, string period)
    {
        if (!IsActive || BillingPeriod == BillingPeriod.OneTime)
            return false;

        if (!DateTime.TryParseExact(
                $"{period}-01",
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var periodStart))
        {
            return false;
        }

        var from = (ServiceStartDate ?? contractStart).UtcDateTime.Date;
        var to = (ServiceEndDate ?? contractEnd).UtcDateTime.Date;

        // Dövr xidmətin (və müqavilənin) qüvvədə olduğu aralığa düşməlidir.
        if (periodStart < from || periodStart > to)
            return false;

        // Rüblük/illik xidmətlər yalnız uyğun aylarda hesablanır: rübün/ilin
        // başlanğıc ayı xidmətin başladığı ayın üzərinə düşür. İllik sığorta
        // beləcə hər il öz ildönümü ayında hesablanır.
        var monthsSinceStart = (periodStart.Year - from.Year) * 12 + (periodStart.Month - from.Month);

        return BillingPeriod switch
        {
            BillingPeriod.Monthly => true,
            BillingPeriod.Quarterly => monthsSinceStart % 3 == 0,
            BillingPeriod.Yearly => monthsSinceStart % 12 == 0,
            _ => false
        };
    }

    private static void Validate(
        string name,
        string unit,
        decimal unitPrice,
        decimal quantity,
        int? paymentTermDays,
        DateTimeOffset? serviceStartDate,
        DateTimeOffset? serviceEndDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Xidmətin adı mütləqdir", nameof(name));
        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Ölçü vahidi mütləqdir", nameof(unit));
        if (unitPrice < 0)
            throw new ArgumentException("Vahid qiymət mənfi ola bilməz", nameof(unitPrice));
        if (quantity <= 0)
            throw new ArgumentException("Miqdar müsbət olmalıdır", nameof(quantity));
        if (paymentTermDays is < 0)
            throw new ArgumentException("Ödəniş müddəti mənfi ola bilməz", nameof(paymentTermDays));
        if (serviceStartDate is not null && serviceEndDate is not null && serviceEndDate < serviceStartDate)
            throw new ArgumentException("Xidmətin bitmə tarixi başlanğıcdan əvvəl ola bilməz", nameof(serviceEndDate));
    }
}
