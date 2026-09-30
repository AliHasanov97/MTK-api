using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges.Events;
using MTK.Modules.Payments.Domain.Rates;

namespace MTK.Modules.Payments.Domain.Charges;

public sealed class Charge : SearchableEntity
{
    private Charge() : base() { }

    public Guid OwnerId { get; private set; }
    public PropertyType PropertyType { get; private set; }
    public Guid PropertyId { get; private set; }
    public string Period { get; private set; } = string.Empty; // "2026-09"

    /// <summary>
    /// Borcun aid olduğu / yarandığı tarix — borcun <b>yaşı</b> budur və ödənişlərin
    /// hansı borca əvvəl tətbiq olunacağını bu müəyyən edir (FIFO). Aylıq borclarda
    /// dövrün ilk günü, birdəfəlik borclarda yaradılma anıdır. <see cref="Period"/>
    /// yalnız identifikatordur — sıralama üçün istifadə olunmur, çünki "MANUAL-…"
    /// kimi dəyərlər aylıq "yyyy-MM" ilə düzgün müqayisə olunmur.
    /// </summary>
    public DateTimeOffset IssuedOn { get; private set; }

    public decimal Amount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public ChargeStatus Status { get; private set; }
    public string? Description { get; private set; } // Human-readable label, mainly for manual/one-off charges

    // Snapshot: Calculation details at the time of charge creation
    public decimal? AreaSquareMeters { get; private set; }  // For apartments only
    public decimal RateAmount { get; private set; }         // Rate used in calculation
    public RateType RateType { get; private set; }          // Rate type used

    /// <summary>Qalıq borc — heç vaxt mənfi olmur.</summary>
    public decimal OutstandingAmount => Amount - PaidAmount;

    public static Charge Create(
        Guid ownerId,
        PropertyType propertyType,
        Guid propertyId,
        string period,
        DateTimeOffset issuedOn,
        decimal amount,
        decimal rateAmount,
        RateType rateType,
        decimal? areaSquareMeters,
        string? description = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Charge amount must be positive");

        var charge = new Charge
        {
            OwnerId = ownerId,
            PropertyType = propertyType,
            PropertyId = propertyId,
            Period = period,
            IssuedOn = issuedOn,
            Amount = amount,
            PaidAmount = 0,
            Status = ChargeStatus.Unpaid,
            RateAmount = rateAmount,
            RateType = rateType,
            AreaSquareMeters = areaSquareMeters,
            Description = description
        };
        charge.SetCreatedAt();
        charge.RaiseDomainEvent(new ChargeCreatedDomainEvent(charge.Id, ownerId, amount, period));
        return charge;
    }

    /// <summary>
    /// Ödənişi borca tətbiq edir. Qalıq borcdan artıq ödəniş cəhdi <b>exception</b>
    /// atır: "borc öz məbləğindən çox ödənilə bilməz" invariantı domendə qorunur
    /// (əvvəllər yalnız çağıran tərəfdəki Math.Min ilə təmin olunurdu, yəni hər
    /// yeni çağırış yeri invariantı poza bilərdi).
    /// </summary>
    public void ApplyPayment(decimal paymentAmount)
    {
        if (paymentAmount <= 0)
            throw new ArgumentException("Payment amount must be positive");

        if (paymentAmount > OutstandingAmount)
            throw new InvalidOperationException(
                $"Ödəniş qalıq borcdan böyükdür (qalıq: {OutstandingAmount}, ödəniş: {paymentAmount})");

        PaidAmount += paymentAmount;

        Status = PaidAmount >= Amount
            ? ChargeStatus.Paid
            : ChargeStatus.PartiallyPaid;

        SetUpdatedAt();
    }

    /// <summary>
    /// Borcu sahibin əvvəlki ödənişindən qalan avansla bağlayır. <see cref="ApplyPayment"/>
    /// ilə eyni qaydada işləyir, əlavə olaraq hansı ödənişin xərcləndiyini bildirən
    /// domain event qaldırır (iz/audit üçün).
    /// </summary>
    public void ApplyAdvanceFrom(Guid paymentId, decimal amount)
    {
        ApplyPayment(amount);
        RaiseDomainEvent(new ChargeSettledFromAdvanceDomainEvent(Id, OwnerId, paymentId, amount));
    }
}
