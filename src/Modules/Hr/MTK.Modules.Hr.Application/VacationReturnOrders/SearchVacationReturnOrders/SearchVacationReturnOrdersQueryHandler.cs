using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.VacationReturnOrders;

namespace MTK.Modules.Hr.Application.VacationReturnOrders.SearchVacationReturnOrders;

internal sealed class SearchVacationReturnOrdersQueryHandler
    : IQueryHandler<SearchVacationReturnOrdersQuery, SearchVacationReturnOrdersResponse>
{
    private readonly IVacationReturnOrderRepository _repository;
    private readonly IMapper _mapper;

    public SearchVacationReturnOrdersQueryHandler(
        IVacationReturnOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchVacationReturnOrdersResponse>> Handle(
        SearchVacationReturnOrdersQuery request,
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

        var data = _mapper.Map<List<SearchVacationReturnOrdersResponseItem>>(orders);
        return Result.Success(new SearchVacationReturnOrdersResponse(data, totalCount, request.Page, request.PageSize));
    }
}
