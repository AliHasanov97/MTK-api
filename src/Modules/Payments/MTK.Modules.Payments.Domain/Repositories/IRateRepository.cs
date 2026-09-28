using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Rates;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IRateRepository : IRepository<Rate>
{
    Task<Rate?> GetCurrentRateAsync(RateType rateType, CancellationToken cancellationToken = default);
    Task<IEnumerable<Rate>> GetActiveRatesAsync(CancellationToken cancellationToken = default);
}
