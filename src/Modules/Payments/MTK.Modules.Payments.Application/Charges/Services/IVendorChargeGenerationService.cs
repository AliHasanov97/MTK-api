namespace MTK.Modules.Payments.Application.Charges.Services;

public interface IVendorChargeGenerationService
{
    /// <summary>
    /// Verilmiş dövr ("yyyy-MM") üçün bütün aktiv müqavilələrin cədvəl üzrə
    /// xidmət borclarını yaradır. Qayıdan dəyər yaradılmış borcların sayıdır.
    /// <paramref name="regenerateCancelled"/> yalnız əl ilə bərpa üçündür: ləğv edilmiş borc "yoxdur" sayılır.
    /// </summary>
    Task<int> GenerateScheduledChargesAsync(
        string period,
        bool regenerateCancelled = false,
        CancellationToken cancellationToken = default);
}
