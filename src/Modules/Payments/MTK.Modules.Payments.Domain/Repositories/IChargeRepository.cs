using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IChargeRepository : IRepository<Charge>
{
    Task<IEnumerable<Charge>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Charge>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Charge>> GetUnpaidChargesAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Charge>> GetUnpaidChargesAsync(Guid ownerId, Guid propertyId, CancellationToken cancellationToken = default);
    Task<bool> ChargeExistsForPeriodAsync(Guid ownerId, Guid propertyId, string period, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sahiblər üzrə ümumi borc cəmi (silinmişlər istisna). Balansın mütləq yenidən
    /// hesablanması üçün — sahib başına ayrı sorğu getməsin deyə toplu işləyir.
    /// </summary>
    Task<Dictionary<Guid, decimal>> GetTotalAmountByOwnerIdsAsync(
        IReadOnlyCollection<Guid> ownerIds, CancellationToken cancellationToken = default);
}
