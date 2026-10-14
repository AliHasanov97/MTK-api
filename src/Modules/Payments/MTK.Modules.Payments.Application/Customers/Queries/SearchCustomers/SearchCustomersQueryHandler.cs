using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Customers.Queries.SearchCustomers;

internal sealed class SearchCustomersQueryHandler : IQueryHandler<SearchCustomersQuery, SearchCustomersResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public SearchCustomersQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<Result<SearchCustomersResponse>> Handle(
        SearchCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _customerRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var items = _mapper.Map<IReadOnlyCollection<CustomerSearchResult>>(customers);

        return new SearchCustomersResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);
    }
}
