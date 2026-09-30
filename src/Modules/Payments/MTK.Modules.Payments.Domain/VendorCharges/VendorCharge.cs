using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Contracts;

namespace MTK.Modules.Payments.Domain.VendorCharges;

/// <summary>
/// Tədarükçü qarşısında borcumuz (payable).
///
/// Sakin borcundan (<c>Charge</c>) ayrı agregatdır: burada sahib/mənzil yoxdur,
/// pul bizdən çıxır. İki yolla yaranır:
/// <list type="bullet">
/// <item>Müqavilə üzrə xidmət cədvəli — <see cref="ForServiceSchedule"/></item>
/// <item>Mal tədarükü (qaimə) — <see cref="ForGoodsDelivery"/></item>
/// </list>
///
/// Borc pul hərəkəti deyil: ledger-ə (Transaction) heç nə yazılmır. Pul yalnız
/// ödəniş edildikdə çıxır (VendorPayment → Expense Transaction).
/// </summary>
public sealed class VendorCharge : SearchableEntity
{
    private VendorCharge() : base() { }

    public Guid ContractId { get; private set; }

    /// <summary>Snapshot — hesabatda müqaviləni yükləmədən istifadə olunur.</summary>
    public Guid VendorId { get; private set; }

    public Guid? ContractServiceId { get; private set; }
    public Guid? ContractGoodsItemId { get; private set; }

    /// <summary>"2026-09" — yalnız cədvəl üzrə yaranan borclarda dolur.</summary>
    public string? Period { get; private set; }

    public string Description { get; private set; } = string.Empty;

    /// <summary>Qaimə/faktura nömrəsi (mal tədarükü və ya tədarükçü ödənişi üçün).</summary>
    public string? Reference { get; private set; }

    // Snapshot: borc yaranan andaki qiymət və miqdar. Müqavilə sonradan
    // dəyişsə də artıq yaranmış borc dəyişmir (Charge.RateAmount prinsipi).
    public decimal UnitPrice { get; private set; }
    public decimal Quantity { get; private set; }

    public decimal Amount { get; private set; }
    public decimal PaidAmount { get; private set; }

    public VendorChargeStatus Status { get; private set; }
    public VendorChargeSource Source { get; private set; }

    /// <summary>Borcun yarandığı tarix.</summary>
    public DateTimeOffset ChargeDate { get; private set; }

    /// <summary>Son ödəniş tarixi (müqavilədəki ödəniş müddəti ilə hesablanır).</summary>
    public DateTimeOffset? DueDate { get; private set; }

    public string Currency { get; private set; } = "AZN";

    public string? CancellationReason { get; private set; }

    public decimal OutstandingAmount => Amount - PaidAmount;

    public bool IsOverdue =>
        Status is VendorChargeStatus.Unpaid or VendorChargeStatus.PartiallyPaid
        && DueDate is not null
        && DueDate.Value.Date < DateTimeOffset.UtcNow.Date;

    /// <summary>Müqavilə üzrə xidmət cədvəlindən yaranan borc (məs. aylıq texniki xidmət).</summary>
    public static VendorCharge ForServiceSchedule(
        Contract contract,
        ContractService service,
        string period,
        DateTimeOffset chargeDate)
    {
        if (string.IsNullOrWhiteSpace(period))
            throw new ArgumentException("Dövr mütləqdir", nameof(period));

        var charge = new VendorCharge
        {
            ContractId = contract.Id,
            VendorId = contract.VendorId,
            ContractServiceId = service.Id,
            Period = period,
            Description = $"{service.Name} — {period} ({service.Quantity} {service.Unit})",
            UnitPrice = service.UnitPrice,
            Quantity = service.Quantity,
            Amount = service.PeriodAmount,
            PaidAmount = 0,
            Status = VendorChargeStatus.Unpaid,
            Source = VendorChargeSource.ServiceSchedule,
            ChargeDate = chargeDate,
            DueDate = service.PaymentTermDays is { } termDays ? chargeDate.AddDays(termDays) : null,
            Currency = contract.Currency
        };

        charge.SetCreatedAt();
        return charge;
    }

    /// <summary>Mal tədarükü (qaimə) üzrə yaranan borc — miqdar tədarük anında bilinir.</summary>
    public static VendorCharge ForGoodsDelivery(
        Contract contract,
        ContractGoodsItem item,
        decimal quantity,
        string? reference,
        DateTimeOffset chargeDate)
    {
        if (quantity <= 0)
            throw new ArgumentException("Tədarük miqdarı müsbət olmalıdır", nameof(quantity));

        var amount = item.UnitPrice * quantity;

        var charge = new VendorCharge
        {
            ContractId = contract.Id,
            VendorId = contract.VendorId,
            ContractGoodsItemId = item.Id,
            Description = $"{item.Name} — {quantity} {item.Unit}",
            Reference = reference,
            UnitPrice = item.UnitPrice,
            Quantity = quantity,
            Amount = amount,
            PaidAmount = 0,
            Status = VendorChargeStatus.Unpaid,
            Source = VendorChargeSource.GoodsDelivery,
            ChargeDate = chargeDate,
            DueDate = item.PaymentTermDays is { } termDays ? chargeDate.AddDays(termDays) : null,
            Currency = contract.Currency
        };

        charge.SetCreatedAt();
        return charge;
    }

    /// <summary>Ödənişi borca tətbiq edir (qismən ödəniş dəstəklənir).</summary>
    public void ApplyPayment(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Ödəniş məbləği müsbət olmalıdır", nameof(amount));
        if (Status == VendorChargeStatus.Cancelled)
            throw new InvalidOperationException("Ləğv edilmiş borca ödəniş tətbiq oluna bilməz");
        if (amount > OutstandingAmount)
            throw new InvalidOperationException(
                $"Ödəniş qalıq borcdan böyükdür (qalıq: {OutstandingAmount})");

        PaidAmount += amount;
        Status = PaidAmount >= Amount ? VendorChargeStatus.Paid : VendorChargeStatus.PartiallyPaid;
        SetUpdatedAt();
    }

    /// <summary>Ödəniş ləğv edildikdə borcu geri açır.</summary>
    public void ReversePayment(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Məbləğ müsbət olmalıdır", nameof(amount));

        PaidAmount = Math.Max(0, PaidAmount - amount);
        Status = PaidAmount <= 0
            ? VendorChargeStatus.Unpaid
            : PaidAmount >= Amount
                ? VendorChargeStatus.Paid
                : VendorChargeStatus.PartiallyPaid;
        SetUpdatedAt();
    }

    /// <summary>
    /// Səhv yaranmış borcu ləğv edir. Borc silinmir — tarixçə qalır, sadəcə
    /// artıq ödəniş tələb etmir.
    /// </summary>
    public void Cancel(string? reason)
    {
        if (Status == VendorChargeStatus.Paid)
            throw new InvalidOperationException("Ödənilmiş borc ləğv edilə bilməz — əvvəlcə ödənişi ləğv edin");

        Status = VendorChargeStatus.Cancelled;
        CancellationReason = reason;
        SetUpdatedAt();
    }
}
