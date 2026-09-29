using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Rates.Queries.GetCurrentRates;

internal sealed class GetCurrentRatesQueryHandler : IQueryHandler<GetCurrentRatesQuery, IReadOnlyCollection<RateResponse>>
{
    private readonly IRateRepository _rateRepository;

    public GetCurrentRatesQueryHandler(IRateRepository rateRepository)
    {
        _rateRepository = rateRepository;
    }

    public async Task<Result<IReadOnlyCollection<RateResponse>>> Handle(
        GetCurrentRatesQuery request,
        CancellationToken cancellationToken)
    {
        var rates = await _rateRepository.GetActiveRatesAsync(cancellationToken);

        var response = rates
            .Select(r => new RateResponse(
                r.Id,
                r.RateType,
                r.Amount,
                r.EffectiveFrom,
                r.EffectiveTo,
                r.Description,
                r.GarageType))
            .ToList();

        return response;
    }
}
