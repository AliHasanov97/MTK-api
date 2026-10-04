using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Apartments;
using MTK.Modules.Payments.Domain.Charges.Events;
using MTK.Modules.Payments.Domain.Contracts;
using MTK.Modules.Payments.Domain.Garages;
using MTK.Modules.Payments.Domain.Owners;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.Vendors;

namespace MTK.Modules.Payments.Domain.Charges;

/// <summary>
/// Borc — həm sakinə (<see cref="OwnerId"/>), həm də tədarükçüyə (<see cref="VendorId"/>)
/// aid ola bilər.
///
/// Əvvəllər bunlar iki ayrı aqreqat idi (<c>Charge</c> və <c>VendorCharge</c>) və
/// demək olar ki, eyni işi görürdü. Birləşdirmə sayəsində ödənişlərin borclara
/// paylanması (allocation / FIFO / avans) hər iki tərəf üçün eyni mexanizmlə işləyir.
///
/// Tərəf <see cref="OwnerId"/>/<see cref="VendorId"/>-dən biri dolu olmaqla ayrılır
/// (əvvəlki <c>PartyType</c>/<c>PartyId</c> discriminator-cütü tamamilə silinib — bu
/// iki həqiqi FK özü-özlüyündə diskriminatordur); eynilə əmlak da
/// <see cref="ApartmentId"/>/<see cref="GarageId"/>-dən biri ilə.
/// </summary>
public sealed class Charge : SearchableEntity
{
    private Charge() : base() { }

    // ---- Tərəf — OwnerId/VendorId-dən məhz biri dolu olur -------------------------

    public Guid? OwnerId { get; private set; }
    public Owner? Owner { get; private set; }

    public Guid? VendorId { get; private set; }
    public Vendor? Vendor { get; private set; }

    // ---- Sakinə xas sahələr (yalnız OwnerId dolu olanda mənalıdır) ----------------

    public Guid? ApartmentId { get; private set; }
    public Apartment? Apartment { get; private set; }

    public Guid? GarageId { get; private set; }
    public Garage? Garage { get; private set; }

    public decimal? AreaSquareMeters { get; private set; }
    public decimal? RateAmount { get; private set; }
    public RateType? RateType { get; private set; }

    // ---- Tədarükçüyə xas sahələr (yalnız VendorId dolu olanda mənalıdır) ----------

    public Guid? ContractId { get; private set; }
    public Guid? ContractServiceId { get; private set; }
    public DateTimeOffset? DueDate { get; private set; }

    // ---- Ortaq sahələr -----------------------------------------------------------

    /// <summary>"2026-09" — aylıq borclarda dövr; manual/tədarük borclarında fərqli.</summary>
    public string? Period { get; private set; }

    /// <summary>
    /// Borcun aid olduğu / yarandığı tarix — borcun <b>yaşı</b> budur və ödənişlərin
    /// hansı borca əvvəl tətbiq olunacağını bu müəyyən edir (FIFO).
    /// </summary>
    public DateTimeOffset IssuedOn { get; private set; }

    public decimal Amount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public ChargeStatus Status { get; private set; }

    /// <summary>İnsan üçün oxunaqlı təsvir (manual/tədarük borcları üçün).</summary>
    public string? Description { get; private set; }

    /// <summary>Qalıq borc — heç vaxt mənfi olmur.</summary>
    public decimal OutstandingAmount => Amount - PaidAmount;

    /// <summary>Gecikmiş tədarükçü borcu (yalnız Vendor tərəfi üçün mənalıdır).</summary>
    public bool IsOverdue =>
        VendorId.HasValue
        && Status is ChargeStatus.Unpaid or ChargeStatus.PartiallyPaid
        && DueDate is not null
        && DueDate.Value.Date < DateTimeOffset.UtcNow.Date;

    // ---- Fabriklər ---------------------------------------------------------------

    /// <summary>Sakin borcu — mənzil üçün (aylıq və ya manual).</summary>
    public static Charge CreateForApartment(
        Guid ownerId,
        Guid apartmentId,
        string period,
        DateTimeOffset issuedOn,
        decimal amount,
        decimal rateAmount,
        RateType rateType,
        decimal? areaSquareMeters,
        string? description = null)
        => CreateOwnerCharge(
            ownerId, apartmentId, garageId: null, period, issuedOn, amount, rateAmount, rateType,
            areaSquareMeters, description);

    /// <summary>Sakin borcu — qaraj üçün (aylıq və ya manual). Qarajın sahəsi olmur.</summary>
    public static Charge CreateForGarage(
        Guid ownerId,
        Guid garageId,
        string period,
        DateTimeOffset issuedOn,
        decimal amount,
        decimal rateAmount,
        RateType rateType,
        string? description = null)
        => CreateOwnerCharge(
            ownerId, apartmentId: null, garageId, period, issuedOn, amount, rateAmount, rateType,
            areaSquareMeters: null, description);

    private static Charge CreateOwnerCharge(
        Guid ownerId,
        Guid? apartmentId,
        Guid? garageId,
        string period,
        DateTimeOffset issuedOn,
        decimal amount,
        decimal rateAmount,
        RateType rateType,
        decimal? areaSquareMeters,
        string? description)
    {
        if (amount <= 0)
            throw new ArgumentException("Charge amount must be positive");

        var charge = new Charge
        {
            OwnerId = ownerId,
            ApartmentId = apartmentId,
            GarageId = garageId,
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
        charge.RaiseDomainEvent(new ChargeCreatedDomainEvent(charge.Id, PartyType.Owner, ownerId, amount, period));
        return charge;
    }

    /// <summary>Müqavilə üzrə xidmət cədvəlindən yaranan tədarükçü borcu (məs. aylıq texniki xidmət).</summary>
    public static Charge ForServiceSchedule(
        Contract contract,
        ContractService service,
        string period,
        DateTimeOffset chargeDate)
    {
        if (string.IsNullOrWhiteSpace(period))
            throw new ArgumentException("Dövr mütləqdir", nameof(period));

        var charge = new Charge
        {
            VendorId = contract.VendorId,
            ContractId = contract.Id,
            ContractServiceId = service.Id,
            Period = period,
            Description = $"{service.Name} — {period}",
            Amount = service.PeriodAmount,
            PaidAmount = 0,
            Status = ChargeStatus.Unpaid,
            IssuedOn = chargeDate,
            DueDate = service.PaymentTermDays is { } termDays ? chargeDate.AddDays(termDays) : null
        };

        charge.SetCreatedAt();
        return charge;
    }

    /// <summary>
    /// Müqavilə üzrə <b>birdəfəlik</b> xidmətdən (BillingPeriod.OneTime) yaranan tək
    /// dəfəlik tədarükçü borcu (məs. təmir, servis çağırışı) — admin konkret xidməti
    /// seçib indi xərc daxil edəndə yaranır, aylıq cədvəl generasiyasından fərqli
    /// olaraq <see cref="Period"/> daşımır (dövri deyil).
    /// </summary>
    public static Charge ForOneTimeService(
        Contract contract,
        ContractService service,
        decimal amount,
        DateTimeOffset chargeDate)
    {
        if (amount <= 0)
            throw new ArgumentException("Charge amount must be positive", nameof(amount));

        var charge = new Charge
        {
            VendorId = contract.VendorId,
            ContractId = contract.Id,
            ContractServiceId = service.Id,
            Description = service.Name,
            Amount = amount,
            PaidAmount = 0,
            Status = ChargeStatus.Unpaid,
            IssuedOn = chargeDate,
            DueDate = service.PaymentTermDays is { } termDays ? chargeDate.AddDays(termDays) : null
        };

        charge.SetCreatedAt();
        return charge;
    }

    // ---- Ödəniş tətbiqi ----------------------------------------------------------

    /// <summary>
    /// Ödənişi borca tətbiq edir. Qalıq borcdan artıq ödəniş cəhdi <b>exception</b>
    /// atır: "borc öz məbləğindən çox ödənilə bilməz" invariantı domendə qorunur.
    /// </summary>
    public void ApplyPayment(decimal paymentAmount)
    {
        if (paymentAmount <= 0)
            throw new ArgumentException("Payment amount must be positive");

        if (Status == ChargeStatus.Cancelled)
            throw new InvalidOperationException("Ləğv edilmiş borca ödəniş tətbiq oluna bilməz");

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
    /// Borcu əvvəlki ödənişdən qalan avansla bağlayır. <see cref="ApplyPayment"/> ilə
    /// eyni qaydada işləyir, əlavə olaraq hansı ödənişin xərcləndiyini bildirən
    /// domain event qaldırır (iz/audit üçün).
    /// </summary>
    public void ApplyAdvanceFrom(Guid paymentId, decimal amount)
    {
        ApplyPayment(amount);
        RaiseDomainEvent(new ChargeSettledFromAdvanceDomainEvent(Id, OwnerId ?? VendorId!.Value, paymentId, amount));
    }

    /// <summary>
    /// Səhv yaranmış borcu ləğv edir. Borc silinmir — tarixçə qalır, sadəcə
    /// artıq ödəniş tələb etmir.
    /// </summary>
    public void Cancel()
    {
        if (Status == ChargeStatus.Paid)
            throw new InvalidOperationException("Ödənilmiş borc ləğv edilə bilməz");

        Status = ChargeStatus.Cancelled;
        SetUpdatedAt();
    }
}
