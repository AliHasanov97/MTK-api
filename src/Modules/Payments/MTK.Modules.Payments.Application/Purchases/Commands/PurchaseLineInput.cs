namespace MTK.Modules.Payments.Application.Purchases.Commands;

/// <summary>Alış sətri yaratmaq/yeniləmək üçün giriş modeli.</summary>
public sealed record PurchaseLineInput(
    Guid NomenclatureId,
    decimal Quantity,
    decimal UnitPrice);
