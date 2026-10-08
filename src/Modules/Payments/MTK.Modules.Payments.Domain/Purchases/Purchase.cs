using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Purchases.Events;

namespace MTK.Modules.Payments.Domain.Purchases;

/// <summary>
/// Alış sənədi (Aggregate Root) — məhsullar bu modulda alınır, sonra
/// <see cref="Receive"/> ilə Warehouse-a ötürülür (stok artımı).
///
/// Vendor və NomenclatureShadow ayrı aggregate-lərdir → FK yox, yalnız Guid
/// saxlanılır (Contract → Vendor konvensiyası). Sətirlər isə bu aggregate-in
/// içindədir → real FK + cascade.
/// </summary>
public sealed class Purchase : SearchableEntity
{
    private readonly List<PurchaseLine> _lines = new();

    private Purchase() : base() { }

    public Guid VendorId { get; private set; }

    /// <summary>Xarici modul istinadı: FK yox, yalnız Guid.</summary>
    public Guid? CreatedByUserId { get; private set; }

    public Guid? ReceivedByUserId { get; private set; }

    public DateTimeOffset PurchaseDate { get; private set; }

    /// <summary>Tədarükçü qaimə/faktura nömrəsi (opsional).</summary>
    public string? InvoiceNumber { get; private set; }

    public string? Note { get; private set; }

    public PurchaseStatus Status { get; private set; }

    /// <summary>Anbara ötürülmə (qəbul) anı.</summary>
    public DateTimeOffset? ReceivedOnUtc { get; private set; }

    /// <summary>
    /// Sətirlərin cəmi. Denormalizə olunub (sətirləri yükləmədən siyahıda göstərmək üçün) —
    /// hər sətir dəyişikliyindən sonra <see cref="RecalculateTotal"/> ilə yenilənir.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    public IReadOnlyCollection<PurchaseLine> Lines => _lines.AsReadOnly();

    public bool IsEditable => Status == PurchaseStatus.Draft;

    public static Purchase Create(
        Guid vendorId,
        DateTimeOffset purchaseDate,
        string? note = null,
        Guid? createdByUserId = null)
    {
        var purchaseId = Guid.NewGuid();
        var purchase = new Purchase
        {
            Id = purchaseId,
            VendorId = vendorId,
            PurchaseDate = purchaseDate,
            InvoiceNumber = $"MTK-{purchaseDate:yyyy}-{purchaseId.ToString("N")[..12].ToUpperInvariant()}",
            Note = note,
            CreatedByUserId = createdByUserId,
            Status = PurchaseStatus.Draft
        };

        purchase.SetCreatedAt();
        return purchase;
    }

    public void Update(
        DateTimeOffset purchaseDate,
        string? note = null)
    {
        EnsureEditable();

        PurchaseDate = purchaseDate;
        Note = note;
        SetUpdatedAt();
    }

    // ------------------------------------------------------------------
    // Sətirlər — yalnız Draft mərhələsində dəyişdirilir. Qəbuldan sonra
    // sətirləri "sakitcə" dəyişmək anbar stoku ilə uçot arasında uyğunsuzluq
    // yaradardı.
    // ------------------------------------------------------------------

    public PurchaseLine AddLine(Guid nomenclatureId, decimal quantity, decimal unitPrice)
    {
        EnsureEditable();

        var line = PurchaseLine.Create(Id, nomenclatureId, quantity, unitPrice);
        _lines.Add(line);
        RecalculateTotal();
        SetUpdatedAt();

        return line;
    }

    public void UpdateLine(Guid lineId, decimal quantity, decimal unitPrice)
    {
        EnsureEditable();

        var line = Line(lineId);
        line.Update(quantity, unitPrice);
        RecalculateTotal();
        SetUpdatedAt();
    }

    public void RemoveLine(Guid lineId)
    {
        EnsureEditable();

        _lines.Remove(Line(lineId));
        RecalculateTotal();
        SetUpdatedAt();
    }

    // ------------------------------------------------------------------
    // Status keçidləri
    // ------------------------------------------------------------------

    /// <summary>
    /// Alışı qəbul edir — Warehouse-a göndəriləcək domain event qaldırılır.
    /// Idempotentdir: artıq qəbul edilibsə heç nə etmir.
    /// </summary>
    public void Receive(DateTimeOffset receivedOnUtc, Guid? receivedByUserId = null)
    {
        if (Status == PurchaseStatus.Received)
            return;

        if (Status == PurchaseStatus.Cancelled)
            throw new InvalidOperationException("Ləğv edilmiş alış qəbul edilə bilməz");

        if (_lines.Count == 0)
            throw new InvalidOperationException("Alışın ən azı bir sətri olmalıdır");

        Status = PurchaseStatus.Received;
        ReceivedOnUtc = receivedOnUtc;
        ReceivedByUserId = receivedByUserId;
        SetUpdatedAt();

        RaiseDomainEvent(new PurchaseReceivedDomainEvent(Id, VendorId, receivedOnUtc));
    }

    public void Cancel(string? note = null)
    {
        if (Status == PurchaseStatus.Cancelled)
            return;

        if (Status == PurchaseStatus.Received)
            throw new InvalidOperationException("Qəbul edilmiş alış ləğv edilə bilməz");

        Status = PurchaseStatus.Cancelled;
        if (!string.IsNullOrWhiteSpace(note))
        {
            Note = note;
        }
        SetUpdatedAt();
    }

    /// <summary>Yalnız hazırlanan (Draft) alış silinir.</summary>
    public void Delete()
    {
        if (Status != PurchaseStatus.Draft)
            throw new InvalidOperationException("Yalnız hazırlanan alış silinə bilər");

        SetDeletedAt();
    }

    private void RecalculateTotal() => TotalAmount = _lines.Sum(l => l.LineTotal);

    private PurchaseLine Line(Guid lineId)
    {
        return _lines.FirstOrDefault(l => l.Id == lineId)
            ?? throw new InvalidOperationException($"Alış sətri tapılmadı: {lineId}");
    }

    private void EnsureEditable()
    {
        if (!IsEditable)
            throw new InvalidOperationException("Yalnız hazırlanan (Draft) alış dəyişdirilə bilər");
    }
}
