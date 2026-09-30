using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Contracts.Queries.SearchContracts;

internal sealed class SearchContractsQueryHandler : IQueryHandler<SearchContractsQuery, SearchContractsResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly IVendorRepository _vendorRepository;

    public SearchContractsQueryHandler(
        IContractRepository contractRepository,
        IVendorRepository vendorRepository)
    {
        _contractRepository = contractRepository;
        _vendorRepository = vendorRepository;
    }

    public async Task<Result<SearchContractsResponse>> Handle(
        SearchContractsQuery request,
        CancellationToken cancellationToken)
    {
        var contracts = await _contractRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _contractRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        // Vendor adları ayrı aggregate-dən gəlir — sətir-sətir sorğu yerinə bir dəfə oxunur.
        var vendorIds = contracts.Select(c => c.VendorId).Distinct().ToList();
        var vendorNames = (await _vendorRepository.ListFromIdsAsync(vendorIds, cancellationToken))
            .ToDictionary(v => v.Id, v => v.Name);

        var items = contracts.Select(c => new ContractListItem(
            c.Id,
            c.Number,
            c.VendorId,
            vendorNames.GetValueOrDefault(c.VendorId),
            c.StartDate,
            c.EndDate,
            c.Status,
            c.Currency,
            c.IsExpired,
            c.Note,
            c.CreatedAt)).ToList();

        var response = new SearchContractsResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return response;
    }
}
