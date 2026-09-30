using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.VendorCharges;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IVendorChargeRepository : IRepository<VendorCharge>
{
    /// <summary>
    /// Bu xidmət üçün həmin dövrdə artıq borc yaranıbmı? Generasiya job-unun
    /// idempotentliyi buna əsaslanır (job təkrar işləsə də borc ikilənmir).
    /// </summary>
    Task<bool> ExistsForPeriodAsync(Guid contractServiceId, string period, CancellationToken cancellationToken = default);

    /// <summary>Bu mal sətri üçün həmin qaimə nömrəsi ilə borc artıq yaranıbmı?</summary>
    Task<bool> ExistsForReferenceAsync(Guid contractGoodsItemId, string reference, CancellationToken cancellationToken = default);

    Task<List<VendorCharge>> ListByVendorAsync(Guid vendorId, CancellationToken cancellationToken = default);

    Task<List<VendorCharge>> ListByContractAsync(Guid contractId, CancellationToken cancellationToken = default);
}
