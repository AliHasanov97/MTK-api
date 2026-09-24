using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Garages;

namespace MTK.Modules.Buildings.Domain.Repositories;

/// <summary>
/// Garage repository interface
/// </summary>
public interface IGarageRepository : IRepository<Garage>
{
    Task<IEnumerable<Garage>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNumberAsync(string garageNumber, CancellationToken cancellationToken = default);
}
