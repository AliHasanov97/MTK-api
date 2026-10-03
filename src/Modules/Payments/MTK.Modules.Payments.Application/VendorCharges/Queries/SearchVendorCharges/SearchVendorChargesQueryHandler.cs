using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.VendorCharges.Queries.SearchVendorCharges;

internal sealed class SearchVendorChargesQueryHandler
    : IQueryHandler<SearchVendorChargesQuery, SearchVendorChargesResponse>
{
    private readonly IVendorChargeRepository _vendorChargeRepository;
    private readonly IMapper _mapper;

    public SearchVendorChargesQueryHandler(IVendorChargeRepository vendorChargeRepository, IMapper mapper)
    {
        _vendorChargeRepository = vendorChargeRepository;
        _mapper = mapper;
    }

    public async Task<Result<SearchVendorChargesResponse>> Handle(
        SearchVendorChargesQuery request,
        CancellationToken cancellationToken)
    {
        var charges = await _vendorChargeRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _vendorChargeRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var items = _mapper.Map<IReadOnlyList<VendorChargeResponse>>(charges);

        return new SearchVendorChargesResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);
    }
}
