using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Apartments;

namespace MTK.Modules.Buildings.Domain.Repositories;

/// <summary>
/// Apartment repository interface
/// </summary>
public interface IApartmentRepository : IRepository<Apartment>
{
    Task<IEnumerable<Apartment>> GetByBuildingIdAsync(Guid buildingId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Apartment>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Apartment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNumberAsync(Guid buildingId, string apartmentNumber, CancellationToken cancellationToken = default);
}
