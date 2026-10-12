namespace MTK.Modules.Payments.Application.Vendors;

/// <summary>
/// Vendor məlumatları response DTO - cache-də saxlanılır
/// Display enrichment zamanı party name göstərmək üçün
/// (məs. "AzərsuKanal", "Bakıelektrikşəbəkə")
/// </summary>
public sealed record VendorResponse(
    Guid Id,
    string Name);
