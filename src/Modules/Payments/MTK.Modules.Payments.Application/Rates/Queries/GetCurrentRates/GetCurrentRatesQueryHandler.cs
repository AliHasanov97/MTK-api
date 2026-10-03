using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Rates.Queries.GetCurrentRates;

internal sealed class GetCurrentRatesQueryHandler : IQueryHandler<GetCurrentRatesQuery, IReadOnlyCollection<RateResponse>>
{
    private readonly IRateRepository _rateRepository;
    private readonly IMapper _mapper;

    public GetCurrentRatesQueryHandler(IRateRepository rateRepository, IMapper mapper)
    {
        _rateRepository = rateRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<RateResponse>>> Handle(
        GetCurrentRatesQuery request,
        CancellationToken cancellationToken)
    {
        var rates = await _rateRepository.GetActiveRatesAsync(cancellationToken);

        return _mapper.Map<List<RateResponse>>(rates);
    }
}
