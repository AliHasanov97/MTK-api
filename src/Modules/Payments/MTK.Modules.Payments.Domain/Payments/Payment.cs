using System.ComponentModel.DataAnnotations.Schema;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments.Events;

namespace MTK.Modules.Payments.Domain.Payments;

/// <summary>
/// Ödəniş — həm sakindən alınan (<see cref="OwnerId"/>), həm də tədarükçüyə verilən
/// (<see cref="VendorId"/>) pulu təmsil edir. Əvvəllər bunlar iki ayrı aqreqat idi
/// (<c>Payment</c> və <c>VendorPayment</c>); indi tək modeldir və pulun borclara
/// paylanması (<c>PaymentAllocation</c>) hər iki tərəf üçün eynidir.
///
/// Tərəf <see cref="OwnerId"/>/<see cref="VendorId"/>-dən biri dolu olmaqla ayrılır
/// (<see cref="Charges.Charge"/> ilə eyni naxış); <see cref="PartyType"/>/
/// <see cref="PropertyType"/> artıq DB sütunu deyil, yalnız FIFO/ledger kimi
/// tərəfə-agnostik ümumi mexanizmlərin rahatlığı üçün bu FK-lərdən hesablanan
/// köməkçi xassələrdir.
/// </summary>
public sealed class Payment : SearchableEntity
{
    private Payment() : base() { }

    public Guid? OwnerId { get; private set; }
    public Guid? VendorId { get; private set; }

    [NotMapped]
    public PartyType PartyType => OwnerId.HasValue ? PartyType.Owner : PartyType.Vendor;

    [NotMapped]
    public Guid PartyId => OwnerId ?? VendorId!.Value;

    public decimal Amount { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTimeOffset PaymentDate { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? Notes { get; private set; }

    // Yalnız sakin ödənişləri üçün: ödəniş tək əmlaka hədəflənə bilər və o zaman
    // yalnız həmin əmlakın açıq borclarına paylanır. Hər ikisi null = ümumi ödəniş
    // (FIFO). ApartmentId/GarageId-dən ən çox biri dolu olur.
    public Guid? ApartmentId { get; private set; }
    public Guid? GarageId { get; private set; }

    [NotMapped]
    public PropertyType? PropertyType =>
        ApartmentId.HasValue ? Charges.PropertyType.Apartment
        : GarageId.HasValue ? Charges.PropertyType.Garage
        : null;

    [NotMapped]
    public Guid? PropertyId => ApartmentId ?? GarageId;

    public static Payment CreateForOwner(
        Guid ownerId,
        decimal amount,
        PaymentMethod paymentMethod,
        DateTimeOffset paymentDate,
        string? notes,
        Guid? apartmentId = null,
        Guid? garageId = null)
        => Create(ownerId: ownerId, vendorId: null, amount, paymentMethod, paymentDate, notes, apartmentId, garageId);

    public static Payment CreateForVendor(
        Guid vendorId,
        decimal amount,
        PaymentMethod paymentMethod,
        DateTimeOffset paymentDate,
        string? notes)
        => Create(ownerId: null, vendorId: vendorId, amount, paymentMethod, paymentDate, notes, apartmentId: null, garageId: null);

    private static Payment Create(
        Guid? ownerId,
        Guid? vendorId,
        decimal amount,
        PaymentMethod paymentMethod,
        DateTimeOffset paymentDate,
        string? notes,
        Guid? apartmentId,
        Guid? garageId)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be positive");

        var payment = new Payment
        {
            OwnerId = ownerId,
            VendorId = vendorId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            PaymentDate = paymentDate,
            Status = PaymentStatus.Pending,
            Notes = notes,
            ApartmentId = apartmentId,
            GarageId = garageId
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
