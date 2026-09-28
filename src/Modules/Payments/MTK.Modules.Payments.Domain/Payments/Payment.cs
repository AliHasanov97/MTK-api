using MTK.Common.Domain.Abstractions;
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

    public static Payment Create(Guid ownerId, decimal amount, PaymentMethod paymentMethod, DateTimeOffset paymentDate, string? reference, string? notes)
    {
        var payment = new Payment
        {
            OwnerId = ownerId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            PaymentDate = paymentDate,
            Status = PaymentStatus.Pending,
            Reference = reference,
            Notes = notes
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
    }

    public void Delete()
    {
        SetDeletedAt();
    }
}