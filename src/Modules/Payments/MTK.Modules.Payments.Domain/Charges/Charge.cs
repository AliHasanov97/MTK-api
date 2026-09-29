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
    public decimal Amount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public ChargeStatus Status { get; private set; }
    public string? Description { get; private set; } // Human-readable label, mainly for manual/one-off charges

    // Snapshot: Calculation details at the time of charge creation
    public decimal? AreaSquareMeters { get; private set; }  // For apartments only
    public decimal RateAmount { get; private set; }         // Rate used in calculation
    public RateType RateType { get; private set; }          // Rate type used

    public static Charge Create(
        Guid ownerId,
        PropertyType propertyType,
        Guid propertyId,
        string period,
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

    public void ApplyPayment(decimal paymentAmount)
    {
        if (paymentAmount <= 0)
            throw new ArgumentException("Payment amount must be positive");

        PaidAmount += paymentAmount;

        Status = PaidAmount >= Amount
            ? ChargeStatus.Paid
            : ChargeStatus.PartiallyPaid;

        SetUpdatedAt();
    }

    /// <summary>
    /// Undoes a previously applied payment allocation (used when a payment gets cancelled).
    /// </summary>
    public void ReversePayment(decimal paymentAmount)
    {
        if (paymentAmount <= 0)
            throw new ArgumentException("Payment amount must be positive");

        PaidAmount = Math.Max(0, PaidAmount - paymentAmount);

        Status = PaidAmount <= 0
            ? ChargeStatus.Unpaid
            : PaidAmount >= Amount
                ? ChargeStatus.Paid
                : ChargeStatus.PartiallyPaid;

        SetUpdatedAt();
    }

    public void Delete()
    {
        SetDeletedAt();
    }
}