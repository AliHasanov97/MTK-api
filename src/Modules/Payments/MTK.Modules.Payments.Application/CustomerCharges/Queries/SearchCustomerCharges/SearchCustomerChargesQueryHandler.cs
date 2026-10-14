using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.CustomerCharges.Queries.SearchCustomerCharges;

internal sealed class SearchCustomerChargesQueryHandler
    : IQueryHandler<SearchCustomerChargesQuery, SearchCustomerChargesResponse>
{
    private readonly ICustomerChargeRepository _customerChargeRepository;
    private readonly IMapper _mapper;

    public SearchCustomerChargesQueryHandler(ICustomerChargeRepository customerChargeRepository, IMapper mapper)
    {
        _customerChargeRepository = customerChargeRepository;
        _mapper = mapper;
    }

    public async Task<Result<SearchCustomerChargesResponse>> Handle(
        SearchCustomerChargesQuery request,
        CancellationToken cancellationToken)
    {
        var charges = await _customerChargeRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _customerChargeRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var items = _mapper.Map<IReadOnlyList<CustomerChargeResponse>>(charges);

        return new SearchCustomerChargesResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);
    }
}
