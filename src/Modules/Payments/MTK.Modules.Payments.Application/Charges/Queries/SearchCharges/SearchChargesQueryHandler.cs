using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Charges.Services;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Charges.Queries.SearchCharges;

internal sealed class SearchChargesQueryHandler : IQueryHandler<SearchChargesQuery, SearchChargesResponse>
{
    private readonly IChargeRepository _chargeRepository;
    private readonly IChargeDisplayEnricher _displayEnricher;
    private readonly IMapper _mapper;

    public SearchChargesQueryHandler(
        IChargeRepository chargeRepository,
        IChargeDisplayEnricher displayEnricher,
        IMapper mapper)
    {
        _chargeRepository = chargeRepository;
        _displayEnricher = displayEnricher;
        _mapper = mapper;
    }

    public async Task<Result<SearchChargesResponse>> Handle(
        SearchChargesQuery request,
        CancellationToken cancellationToken)
    {
        var charges = await _chargeRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _chargeRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var display = await _displayEnricher.ResolveAsync(charges, cancellationToken);

        var items = _mapper.Map<List<ChargeSearchResult>>(charges)
            .Select(r => r with
            {
                PartyName = display.GetValueOrDefault(r.Id)?.PartyName,
                PropertyLabel = display.GetValueOrDefault(r.Id)?.PropertyLabel,
            })
            .ToList();

        var response = new SearchChargesResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return response;
    }
}
