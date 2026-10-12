namespace MTK.Modules.Payments.Application.Garages;

/// <summary>
/// Garage məlumatları response DTO - cache-də saxlanılır
/// Display enrichment zamanı property label göstərmək üçün
/// (məs. "Qaraj 15")
/// </summary>
public sealed record GarageResponse(
    Guid Id,
    string GarageNumber);
