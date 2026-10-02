using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments.Events;

namespace MTK.Modules.Payments.Domain.Payments;

/// <summary>
/// Ödəniş — həm sakindən alınan (<see cref="PartyType.Owner"/>), həm də tədarükçüyə
/// verilən (<see cref="PartyType.Vendor"/>) pulu təmsil edir. Əvvəllər bunlar iki ayrı
/// aqreqat idi (<c>Payment</c> və <c>VendorPayment</c>); indi tək modeldir və pulun
/// borclara paylanması (<c>PaymentAllocation</c>) hər iki tərəf üçün eynidir.
/// </summary>
public sealed class Payment : SearchableEntity
{
    private Payment() : base() { }

    public PartyType PartyType { get; private set; }

    /// <summary>Sahibin və ya tədarükçünün Id-si (tərəfə uyğun).</summary>
    public Guid PartyId { get; private set; }

    public decimal Amount { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTimeOffset PaymentDate { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? Notes { get; private set; }

    // Yalnız sakin ödənişləri üçün: ödəniş tək əmlaka hədəflənə bilər və o zaman
    // yalnız həmin əmlakın açıq borclarına paylanır. Null = ümumi ödəniş (FIFO).
    public Guid? PropertyId { get; private set; }
    public PropertyType? PropertyType { get; private set; }

    public static Payment Create(
        PartyType partyType,
        Guid partyId,
        decimal amount,
        PaymentMethod paymentMethod,
        DateTimeOffset paymentDate,
        string? notes,
        Guid? propertyId = null,
        PropertyType? propertyType = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be positive");

        var payment = new Payment
        {
            PartyType = partyType,
            PartyId = partyId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            PaymentDate = paymentDate,
            Status = PaymentStatus.Pending,
            Notes = notes,
            PropertyId = propertyId,
            PropertyType = propertyType
        };
        payment.SetCreatedAt();
        return payment;
    }

    /// <summary>
    /// Ödənişi tamamlayır. Ledger qeydi bu event-in handler-ində yazılır — tərəfə
    /// görə gəlir (sakin) və ya xərc (tədarükçü) qeydi.
    ///
    /// Ödəniş <b>geri qaytarılmır/ləğv edilmir</b>: tamamlanmış pul hərəkəti artıq
    /// ledger-ə düşüb. Səhv ödəniş zərurət yaranarsa ayrı düzəliş (əks) əməliyyatı
    /// kimi modelləşdirilməlidir, mövcud ödənişin üzərində "reversal" kimi yox.
    /// </summary>
    public void MarkAsCompleted()
    {
        Status = PaymentStatus.Completed;
        SetUpdatedAt();
        RaiseDomainEvent(new PaymentCompletedDomainEvent(Id, PartyType, PartyId, Amount));
    }
}
