using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Contracts.Queries.SearchContracts;

internal sealed class SearchContractsQueryHandler : IQueryHandler<SearchContractsQuery, SearchContractsResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly ICustomerRepository _customerRepository;

    public SearchContractsQueryHandler(
        IContractRepository contractRepository,
        IVendorRepository vendorRepository,
        ICustomerRepository customerRepository)
    {
        _contractRepository = contractRepository;
        _vendorRepository = vendorRepository;
        _customerRepository = customerRepository;
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
        var vendorIds = contracts.Where(c => c.VendorId.HasValue).Select(c => c.VendorId!.Value).Distinct().ToList();
        var vendorNames = (await _vendorRepository.ListFromIdsAsync(vendorIds, cancellationToken))
            .ToDictionary(v => v.Id, v => v.Name);
        var customerIds = contracts.Where(c => c.CustomerId.HasValue).Select(c => c.CustomerId!.Value).Distinct().ToList();
        var customerNames = (await _customerRepository.ListFromIdsAsync(customerIds, cancellationToken))
            .ToDictionary(c => c.Id, c => c.Name);

        var items = contracts.Select(c => new ContractListItem(
            c.Id,
            c.Number,
            c.VendorId,
            c.VendorId.HasValue ? vendorNames.GetValueOrDefault(c.VendorId.Value) : null,
            c.CustomerId,
            c.CustomerId.HasValue ? customerNames.GetValueOrDefault(c.CustomerId.Value) : null,
            c.StartDate,
            c.EndDate,
            c.Status,
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
