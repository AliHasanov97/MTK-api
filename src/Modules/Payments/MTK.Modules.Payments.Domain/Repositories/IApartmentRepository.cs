using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Apartments;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IApartmentRepository : IRepository<Apartment>
{
    /// <summary>Batch lookup with Building included — avoids N+1 when labelling many payments at once.</summary>
    Task<List<Apartment>> ListFromIdsWithBuildingAsync(List<Guid> ids, CancellationToken cancellationToken = default);
}
