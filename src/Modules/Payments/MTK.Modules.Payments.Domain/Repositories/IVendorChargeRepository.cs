using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Domain.Repositories;

/// <summary>
/// Tədarükçüyə aid borclar üzrə görünüş. Borclar artıq tək <see cref="Charge"/>
/// aqreqatındadır; bu repository yalnız <c>PartyType == Vendor</c> sətirləri üzərində
/// işləyir (sakin repository-si isə Owner tərəfi üzərində).
/// </summary>
public interface IVendorChargeRepository : IRepository<Charge>
{
    /// <summary>
    /// Bu xidmət üçün həmin dövrdə artıq borc yaranıbmı? Generasiya job-unun
    /// idempotentliyi buna əsaslanır (job təkrar işləsə də borc ikilənmir).
    /// </summary>
    Task<bool> ExistsForPeriodAsync(Guid contractServiceId, string period, CancellationToken cancellationToken = default);

    Task<List<Charge>> ListByVendorAsync(Guid vendorId, CancellationToken cancellationToken = default);

    Task<List<Charge>> ListByContractAsync(Guid contractId, CancellationToken cancellationToken = default);

    /// <summary>Tədarükçünün açıq borcları, FIFO sırası ilə (ödəniş paylanması üçün).</summary>
    Task<List<Charge>> GetUnpaidChargesAsync(Guid vendorId, CancellationToken cancellationToken = default);
}
