using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions.Events;

namespace MTK.Modules.Warehouse.Domain.WarehouseTransactions;

/// <summary>
/// Anbar əməliyyatı - mal gəlişi və ya çıxışı
/// </summary>
public sealed class WarehouseTransaction : Entity
{
    private WarehouseTransaction(
        Guid id,
        TransactionType transactionType,
        Guid nomenclatureId,
        decimal quantity,
        decimal? unitPrice,
        DateTimeOffset transactionDate,
        string? referenceType,
        Guid? referenceId,
        string? notes,
        Guid? createdByUserId) : base(id)
    {
        TransactionType = transactionType;
        NomenclatureId = nomenclatureId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = unitPrice.HasValue ? quantity * unitPrice.Value : null;
        TransactionDate = transactionDate;
        ReferenceType = referenceType;
        ReferenceId = referenceId;
        Notes = notes;
        CreatedByUserId = createdByUserId;
    }

    // Private constructor for EF Core
    private WarehouseTransaction() : base()
    {
    }

    /// <summary>
    /// Əməliyyat tipi (Daxilolma/Çıxarış)
    /// </summary>
    public TransactionType TransactionType { get; private set; }

    /// <summary>
    /// Nomenklatura ID
    /// </summary>
    public Guid NomenclatureId { get; private set; }

    /// <summary>
    /// Nomenklatura (navigation property)
    /// </summary>
    public Nomenclature Nomenclature { get; private set; } = null!;

    /// <summary>
    /// Miqdar
    /// </summary>
    public decimal Quantity { get; private set; }

    /// <summary>
    /// Vahid qiymət (opsional, əsasən mal qəbulunda olur)
    /// </summary>
    public decimal? UnitPrice { get; private set; }

    /// <summary>
    /// Ümumi qiymət (calculated: Quantity * UnitPrice)
    /// </summary>
    public decimal? TotalPrice { get; private set; }

    /// <summary>
    /// Əməliyyat tarixi
    /// </summary>
    public DateTimeOffset TransactionDate { get; private set; }

    /// <summary>
    /// İstinad tipi (məs: "AccountingPurchase", "ManualIssue")
    /// </summary>
    public string? ReferenceType { get; private set; }

    /// <summary>
    /// İstinad ID (məs: Accounting modulundakı Purchase ID)
    /// </summary>
    public Guid? ReferenceId { get; private set; }

    /// <summary>
    /// Qeydlər
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Əməliyyatı edən istifadəçi ID
    /// </summary>
    public Guid? CreatedByUserId { get; private set; }

    /// <summary>
    /// Mal qəbulu qeyd edir
    /// </summary>
    public static WarehouseTransaction RecordReceipt(
        Guid nomenclatureId,
        decimal quantity,
        decimal? unitPrice = null,
        DateTimeOffset? transactionDate = null,
        string? referenceType = null,
        Guid? referenceId = null,
        string? notes = null,
        Guid? createdByUserId = null)
    {
        var transaction = new WarehouseTransaction(
            Guid.NewGuid(),
            TransactionType.Receipt,
            nomenclatureId,
            quantity,
            unitPrice,
            transactionDate ?? DateTimeOffset.UtcNow,
            referenceType,
            referenceId,
            notes,
            createdByUserId);

        transaction.SetCreatedAt();

        transaction.RaiseDomainEvent(new WarehouseReceiptRecordedDomainEvent(
            transaction.Id,
            nomenclatureId,
            quantity));

        return transaction;
    }

    /// <summary>
    /// Mal çıxarışı qeyd edir
    /// </summary>
    public static WarehouseTransaction RecordIssue(
        Guid nomenclatureId,
        decimal quantity,
        DateTimeOffset? transactionDate = null,
        string? referenceType = null,
        Guid? referenceId = null,
        string? notes = null,
        Guid? createdByUserId = null)
    {
        var transaction = new WarehouseTransaction(
            Guid.NewGuid(),
            TransactionType.Issue,
            nomenclatureId,
            quantity,
            null, // Issue-də adətən unitPrice olmur
            transactionDate ?? DateTimeOffset.UtcNow,
            referenceType,
            referenceId,
            notes,
            createdByUserId);

        transaction.SetCreatedAt();

        transaction.RaiseDomainEvent(new WarehouseIssueRecordedDomainEvent(
            transaction.Id,
            nomenclatureId,
            quantity));

        return transaction;
    }

    /// <summary>
    /// Qeydləri yeniləyir
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        SetUpdatedAt();
    }
}
