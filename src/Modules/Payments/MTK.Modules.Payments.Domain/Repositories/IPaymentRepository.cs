using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tərəfə (sakin və ya tədarükçü) aid bütün ödənişlər. Avansın yüklənməsi hər
    /// iki tərəf üçün eyni mexanizmlə işləsin deyə ümumi metoddur.
    /// </summary>
    Task<IEnumerable<Payment>> GetByPartyIdAsync(PartyType partyType, Guid partyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sahiblər üzrə tamamlanmış ödənişlərin cəmi (yalnız Owner tərəfi). Ləğv
    /// edilmiş/icra olunmamış ödənişlər daxil edilmir.
    /// </summary>
    Task<Dictionary<Guid, decimal>> GetCompletedTotalByOwnerIdsAsync(
        IReadOnlyCollection<Guid> ownerIds, CancellationToken cancellationToken = default);
}
