namespace MTK.Modules.Payments.Application.Owners;

/// <summary>
/// Owner məlumatları response DTO - cache-də saxlanılır
/// Display enrichment (charge/payment detail-də owner adı göstərmək) üçün istifadə olunur
/// </summary>
public sealed record OwnerResponse(
    Guid Id,
    string FullName);
