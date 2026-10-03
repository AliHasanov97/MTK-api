using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Charges.Queries.SearchCharges;

internal sealed class SearchChargesQueryHandler : IQueryHandler<SearchChargesQuery, SearchChargesResponse>
{
    private readonly IChargeRepository _chargeRepository;
    private readonly IMapper _mapper;

    public SearchChargesQueryHandler(IChargeRepository chargeRepository, IMapper mapper)
    {
        _chargeRepository = chargeRepository;
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

        var items = _mapper.Map<IReadOnlyCollection<ChargeSearchResult>>(charges);

        var response = new SearchChargesResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return response;
    }
}
