namespace MTK.Modules.Payments.Application.Charges.Services;

/// <summary>Sakin borclarının generasiyası nəticəsi.</summary>
/// <param name="Created">Yaradılmış borcların sayı</param>
/// <param name="ApartmentsSkippedNoRate">Mənzil tarifi (PerSquareMeter) olmadığı üçün borc yazılmayan mənzillərin sayı</param>
/// <param name="GaragesSkippedNoRate">Qaraj tarifi olmadığı üçün borc yazılmayan qarajların sayı</param>
public sealed record ChargeGenerationResult(int Created, int ApartmentsSkippedNoRate, int GaragesSkippedNoRate);

public interface IChargeGenerationService
{
    /// <summary>
    /// Verilmiş dövr ("yyyy-MM") üçün hələ borcu olmayan mənzil/qarajlara borc yaradır (idempotent).
    /// <paramref name="regenerateCancelled"/> true olarsa ləğv edilmiş borclar "yoxdur" sayılır və əmlak üçün yenidən yaradılır
    /// (yalnız əl ilə bərpa üçün; planlı iş ləğv edilmiş borcu təkrar yaratmır).
    /// </summary>
    Task<ChargeGenerationResult> GenerateMonthlyChargesAsync(
        string period,
        bool regenerateCancelled = false,
        CancellationToken cancellationToken = default);
}
