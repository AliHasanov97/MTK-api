using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.OwnershipHistories;

namespace MTK.Modules.Buildings.Domain.Repositories;

/// <summary>
/// OwnershipHistory repository interface
/// </summary>
public interface IOwnershipHistoryRepository : IRepository<OwnershipHistory>
{
    Task<IEnumerable<OwnershipHistory>> GetByApartmentIdAsync(Guid apartmentId, CancellationToken cancellationToken = default);
    Task<OwnershipHistory?> GetCurrentOwnershipAsync(Guid apartmentId, CancellationToken cancellationToken = default);
}
