using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Parties;

namespace MTK.Modules.Payments.Domain.Repositories;

/// <summary>
/// Sakinə aid borclar üzrə oxuma/yazma. Tədarükçü borcları eyni <see cref="Charge"/>
/// aqreqatındadır, lakin onlara <see cref="IVendorChargeRepository"/> vasitəsilə
/// müraciət olunur (tərəfə görə scope).
/// </summary>
public interface IChargeRepository : IRepository<Charge>
{
    Task<IEnumerable<Charge>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Charge>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Charge>> GetUnpaidChargesAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Charge>> GetUnpaidChargesAsync(Guid ownerId, Guid propertyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tərəfə (sakin və ya tədarükçü) aid açıq borclar, FIFO sırası ilə. Ödəniş
    /// paylanması hər iki tərəf üçün eyni mexanizmdən istifadə etsin deyə ümumi
    /// metoddur.
    /// </summary>
    Task<IEnumerable<Charge>> GetUnpaidChargesByPartyAsync(PartyType partyType, Guid partyId, CancellationToken cancellationToken = default);

    Task<bool> ChargeExistsForPeriodAsync(Guid ownerId, Guid propertyId, string period, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sahiblər üzrə ümumi borc cəmi (silinmişlər istisna, yalnız Owner tərəfi).
    /// Balansın mütləq yenidən hesablanması üçün — sahib başına ayrı sorğu getməsin
    /// deyə toplu işləyir.
    /// </summary>
    Task<Dictionary<Guid, decimal>> GetTotalAmountByOwnerIdsAsync(
        IReadOnlyCollection<Guid> ownerIds, CancellationToken cancellationToken = default);
}
