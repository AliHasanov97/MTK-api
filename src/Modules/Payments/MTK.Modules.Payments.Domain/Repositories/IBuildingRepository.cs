using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Buildings;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IBuildingRepository : IRepository<Building>
{
    /// <summary>
    /// The complex's one building — garages aren't tied to a specific building in
    /// Buildings' own domain (see Garage, which carries no BuildingId at all), so a
    /// garage payment's receipt address falls back to this rather than a per-unit lookup.
    /// </summary>
    Task<Building?> GetFirstAsync(CancellationToken cancellationToken = default);
}
