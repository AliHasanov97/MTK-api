using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.SearchEmploymentStatusChangeOrders;

internal sealed class SearchEmploymentStatusChangeOrdersQueryHandler
    : IQueryHandler<SearchEmploymentStatusChangeOrdersQuery, SearchEmploymentStatusChangeOrdersResponse>
{
    private readonly IEmploymentStatusChangeOrderRepository _repository;
    private readonly IMapper _mapper;

    public SearchEmploymentStatusChangeOrdersQueryHandler(
        IEmploymentStatusChangeOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchEmploymentStatusChangeOrdersResponse>> Handle(
        SearchEmploymentStatusChangeOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await _repository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _repository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var data = _mapper.Map<List<SearchEmploymentStatusChangeOrdersResponseItem>>(orders);
        return Result.Success(new SearchEmploymentStatusChangeOrdersResponse(data, totalCount, request.Page, request.PageSize));
    }
}
