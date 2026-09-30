using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Vendors.Queries.SearchVendors;

internal sealed class SearchVendorsQueryHandler : IQueryHandler<SearchVendorsQuery, SearchVendorsResponse>
{
    private readonly IVendorRepository _vendorRepository;

    public SearchVendorsQueryHandler(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }

    public async Task<Result<SearchVendorsResponse>> Handle(
        SearchVendorsQuery request,
        CancellationToken cancellationToken)
    {
        var vendors = await _vendorRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _vendorRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var items = vendors.Select(v => new VendorSearchResult(
            v.Id,
            v.Name,
            v.VendorType,
            v.Voen,
            v.Director,
            v.Email,
            v.Phone,
            v.Address,
            v.IsActive,
            v.CreatedAt)).ToList();

        var response = new SearchVendorsResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return response;
    }
}
