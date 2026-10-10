using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.EmploymentOrders;

namespace MTK.Modules.Hr.Application.EmploymentOrders.SearchEmploymentOrders;

internal sealed class SearchEmploymentOrdersQueryHandler : IQueryHandler<SearchEmploymentOrdersQuery, SearchEmploymentOrdersResponse>
{
    private readonly IEmploymentOrderRepository _employmentOrderRepository;
    private readonly IMapper _mapper;

    public SearchEmploymentOrdersQueryHandler(
        IEmploymentOrderRepository employmentOrderRepository,
        IMapper mapper)
    {
        _employmentOrderRepository = employmentOrderRepository;
        _mapper = mapper;
    }

    public async Task<Result<SearchEmploymentOrdersResponse>> Handle(SearchEmploymentOrdersQuery request, CancellationToken cancellationToken)
    {
        var employmentOrders = await _employmentOrderRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _employmentOrderRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var data = _mapper.Map<List<SearchEmploymentOrdersResponseItem>>(employmentOrders);
        return Result.Success(new SearchEmploymentOrdersResponse(data, totalCount, request.Page, request.PageSize));
    }
}
