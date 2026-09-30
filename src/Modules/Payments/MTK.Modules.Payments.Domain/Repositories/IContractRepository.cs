using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Contracts;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IContractRepository : IRepository<Contract>
{
    /// <summary>Xidmətləri ilə birlikdə bir müqavilə.</summary>
    Task<Contract?> GetWithServicesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dövr üzrə borc generasiyası üçün: qüvvədə olan, xidmətləri yüklənmiş müqavilələr.
    /// </summary>
    Task<List<Contract>> ListActiveWithServicesAsync(CancellationToken cancellationToken = default);
}
