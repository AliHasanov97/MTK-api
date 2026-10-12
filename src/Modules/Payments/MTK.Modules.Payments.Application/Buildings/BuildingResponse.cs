namespace MTK.Modules.Payments.Application.Buildings;

/// <summary>
/// Building məlumatları response DTO - cache-də saxlanılır
/// Display enrichment zamanı apartment label-lərdə building name göstərmək üçün
/// (məs. "Mənzil 5 — Yasamal Residence")
/// </summary>
public sealed record BuildingResponse(
    Guid Id,
    string Name,
    string Address);
