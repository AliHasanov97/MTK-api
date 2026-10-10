using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.VacationOrders;

namespace MTK.Modules.Hr.Application.VacationOrders.SearchVacationOrders;

internal sealed class SearchVacationOrdersQueryHandler
    : IQueryHandler<SearchVacationOrdersQuery, SearchVacationOrdersResponse>
{
    private readonly IVacationOrderRepository _repository;
    private readonly IMapper _mapper;

    public SearchVacationOrdersQueryHandler(
        IVacationOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchVacationOrdersResponse>> Handle(
        SearchVacationOrdersQuery request,
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

        var data = _mapper.Map<List<SearchVacationOrdersResponseItem>>(orders);
        return Result.Success(new SearchVacationOrdersResponse(data, totalCount, request.Page, request.PageSize));
    }
}