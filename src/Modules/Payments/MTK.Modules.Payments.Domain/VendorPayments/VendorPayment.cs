using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.VendorPayments.Events;

namespace MTK.Modules.Payments.Domain.VendorPayments;

/// <summary>
/// Tədarükçüyə edilən ödəniş — borcun bağlanması.
///
/// Sakin ödənişi ilə eyni məntiq: pul hərəkəti domain event ilə ledger-ə yazılır
/// (<c>VendorPaymentCompletedDomainEventHandler</c> → Expense Transaction), borcun
/// özü isə <c>ApplyPayment</c> ilə bağlanır.
/// </summary>
public sealed class VendorPayment : SearchableEntity
{
    private VendorPayment() : base() { }

    public Guid VendorId { get; private set; }

    public Guid VendorChargeId { get; private set; }

    public decimal Amount { get; private set; }

    /// <summary>
    /// Nağd / Bank köçürməsi / Kart — sakin ödənişləri ilə eyni dəyərlər
    /// olduğu üçün eyni enum təkrar istifadə olunur.
    /// </summary>
    public PaymentMethod PaymentMethod { get; private set; }

    public DateTimeOffset PaymentDate { get; private set; }

    public VendorPaymentStatus Status { get; private set; }

    /// <summary>Ödəniş sənədi / qaimə nömrəsi.</summary>
    public string? Reference { get; private set; }

    public string? Notes { get; private set; }

    public static VendorPayment Create(
        Guid vendorId,
        Guid vendorChargeId,
        decimal amount,
        PaymentMethod paymentMethod,
        DateTimeOffset paymentDate,
        string? reference = null,
        string? notes = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Ödəniş məbləği müsbət olmalıdır", nameof(amount));

        var payment = new VendorPayment
        {
            Id = Guid.NewGuid(),
            VendorId = vendorId,
            VendorChargeId = vendorChargeId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            PaymentDate = paymentDate,
            Status = VendorPaymentStatus.Pending,
            Reference = reference,
            Notes = notes
        };

        payment.SetCreatedAt();
        return payment;
    }

    /// <summary>
    /// Ödənişi tamamlayır. Ledger qeydi bu event-in handler-ində yazılır —
    /// borcun bağlanması ilə pulun çıxması bir yerdə qalsın deyə.
    /// </summary>
    public void MarkAsCompleted()
    {
        Status = VendorPaymentStatus.Completed;
        SetUpdatedAt();
        RaiseDomainEvent(new VendorPaymentCompletedDomainEvent(Id, VendorChargeId, VendorId, Amount));
    }

    public void Delete()
    {
        SetDeletedAt();
    }
}
