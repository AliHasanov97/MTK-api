using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Payments.Events;

namespace MTK.Modules.Payments.Domain.Payments;

public sealed class Payment : SearchableEntity
{
    private Payment() : base() { }

    public Guid OwnerId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTimeOffset PaymentDate { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }

    // Optional: when set, this payment is scoped to a single property and is only
    // allocated against that property's unpaid charges (instead of FIFO across the
    // owner's whole debt). Null means "general payment", FIFO across everything.
    public Guid? PropertyId { get; private set; }
    public PropertyType? PropertyType { get; private set; }

    public static Payment Create(
        Guid ownerId,
        decimal amount,
        PaymentMethod paymentMethod,
        DateTimeOffset paymentDate,
        string? reference,
        string? notes,
        Guid? propertyId = null,
        PropertyType? propertyType = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be positive");

        var payment = new Payment
        {
            OwnerId = ownerId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            PaymentDate = paymentDate,
            Status = PaymentStatus.Pending,
            Reference = reference,
            Notes = notes,
            PropertyId = propertyId,
            PropertyType = propertyType
        };
        payment.SetCreatedAt();
        return payment;
    }

    public void MarkAsCompleted()
    {
        Status = PaymentStatus.Completed;
        SetUpdatedAt();
        RaiseDomainEvent(new PaymentCompletedDomainEvent(Id, OwnerId, Amount));
    }

    public void Cancel()
    {
        Status = PaymentStatus.Cancelled;
        SetUpdatedAt();
        RaiseDomainEvent(new PaymentCancelledDomainEvent(Id, OwnerId, Amount));
    }

    public void Delete()
    {
        SetDeletedAt();
    }
}