using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.ApartmentTypes;

namespace MTK.Modules.Buildings.Domain.Repositories;

public interface IApartmentTypeRepository : IRepository<ApartmentType>
{
    Task<IEnumerable<ApartmentType>> GetByBuildingIdAsync(Guid buildingId, CancellationToken cancellationToken = default);
}
