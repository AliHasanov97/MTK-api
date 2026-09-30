using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sahiblər üzrə tamamlanmış ödənişlərin cəmi. Ləğv edilmiş/icra olunmamış
    /// ödənişlər daxil edilmir.
    /// </summary>
    Task<Dictionary<Guid, decimal>> GetCompletedTotalByOwnerIdsAsync(
        IReadOnlyCollection<Guid> ownerIds, CancellationToken cancellationToken = default);
}
