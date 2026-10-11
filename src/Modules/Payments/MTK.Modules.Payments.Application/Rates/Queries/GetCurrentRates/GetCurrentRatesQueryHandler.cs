using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Rates.Services;

namespace MTK.Modules.Payments.Application.Rates.Queries.GetCurrentRates;

internal sealed class GetCurrentRatesQueryHandler : IQueryHandler<GetCurrentRatesQuery, IReadOnlyCollection<RateResponse>>
{
    private readonly ICachedRateService _cachedRateService;

    public GetCurrentRatesQueryHandler(ICachedRateService cachedRateService)
    {
        _cachedRateService = cachedRateService;
    }

    public async Task<Result<IReadOnlyCollection<RateResponse>>> Handle(
        GetCurrentRatesQuery request,
        CancellationToken cancellationToken)
    {
        // Get active rates from cache (30-minute TTL)
        IReadOnlyCollection<RateResponse> rates = await _cachedRateService.GetActiveRatesAsync(cancellationToken);

        return Result.Success(rates);
    }
}
