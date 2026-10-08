namespace MTK.Modules.Payments.Domain.Purchases;

public enum PurchaseStatus
{
    /// <summary>Hazırlanır — sətirlər dəyişdirilə bilər, anbara heç nə getmir.</summary>
    Draft,

    /// <summary>Qəbul edilib — Warehouse-a göndərilib, stok artırılıb.</summary>
    Received,

    /// <summary>Ləğv edilib — heç vaxt anbara getməyib / geri alınıb.</summary>
    Cancelled
}
