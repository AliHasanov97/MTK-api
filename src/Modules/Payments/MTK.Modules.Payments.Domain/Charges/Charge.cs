using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges.Events;

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

    public static Charge Create(Guid ownerId, PropertyType propertyType, Guid propertyId, string period, decimal amount)
    {
        var charge = new Charge
        {
            OwnerId = ownerId,
            PropertyType = propertyType,
            PropertyId = propertyId,
            Period = period,
            Amount = amount,
            PaidAmount = 0,
            Status = ChargeStatus.Unpaid
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

    public void Delete()
    {
        SetDeletedAt();
    }
}