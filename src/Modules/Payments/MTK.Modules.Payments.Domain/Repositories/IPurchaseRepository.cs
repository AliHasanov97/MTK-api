using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Purchases;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IPurchaseRepository : IRepository<Purchase>
{
    /// <summary>Sətirləri ilə birlikdə alışı oxuyur.</summary>
    Task<Purchase?> GetWithLinesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Purchase>> GetReceivedByNomenclatureIdAsync(Guid nomenclatureId, CancellationToken cancellationToken = default);
}
