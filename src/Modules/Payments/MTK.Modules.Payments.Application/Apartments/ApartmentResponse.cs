namespace MTK.Modules.Payments.Application.Apartments;

/// <summary>
/// Apartment məlumatları response DTO - cache-də saxlanılır
/// Display enrichment zamanı property label göstərmək üçün
/// (məs. "Mənzil 15 — Yasamal Residence")
/// BuildingId building cache-dən yükləmək üçün saxlanılır
/// </summary>
public sealed record ApartmentResponse(
    Guid Id,
    Guid BuildingId,
    string ApartmentNumber);
