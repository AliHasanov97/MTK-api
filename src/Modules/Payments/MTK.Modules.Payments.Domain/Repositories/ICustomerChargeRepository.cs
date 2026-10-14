using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Domain.Repositories;

/// <summary>
/// Müştəriyə aid borclar üzrə görünüş. Borclar tək <see cref="Charge"/> aqreqatındadır;
/// bu repository yalnız <c>CustomerId != null</c> sətirləri üzərində işləyir.
/// </summary>
public interface ICustomerChargeRepository : IRepository<Charge>
{
    /// <summary>
    /// Verilmiş dövr üçün borcu yaranmış müqavilə xidmətlərinin ID-ləri (müştəri tərəfi) — bir sorğu ilə.
    /// </summary>
    Task<HashSet<Guid>> GetBilledServiceIdsForPeriodAsync(string period, bool includeCancelled, CancellationToken cancellationToken = default);

    Task<List<Charge>> ListByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
}
