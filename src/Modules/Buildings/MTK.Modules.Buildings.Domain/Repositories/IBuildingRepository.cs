using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Buildings;

namespace MTK.Modules.Buildings.Domain.Repositories;

/// <summary>
/// Building repository interface
/// </summary>
public interface IBuildingRepository : IRepository<Building>
{
    Task<IEnumerable<Building>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}
