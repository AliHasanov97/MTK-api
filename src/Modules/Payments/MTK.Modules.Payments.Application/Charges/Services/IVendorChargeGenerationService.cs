namespace MTK.Modules.Payments.Application.Charges.Services;

public interface IVendorChargeGenerationService
{
    /// <summary>
    /// Verilmiş dövr ("yyyy-MM") üçün bütün aktiv müqavilələrin cədvəl üzrə
    /// xidmət borclarını yaradır. Qayıdan dəyər yaradılmış borcların sayıdır.
    /// </summary>
    Task<int> GenerateScheduledChargesAsync(string period, CancellationToken cancellationToken = default);
}
