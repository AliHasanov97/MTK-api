using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.CompensationOrders;

namespace MTK.Modules.Hr.Application.CompensationOrders.SearchCompensationOrders;

internal sealed class SearchCompensationOrdersQueryHandler
    : IQueryHandler<SearchCompensationOrdersQuery, SearchCompensationOrdersResponse>
{
    private readonly ICompensationOrderRepository _repository;
    private readonly IMapper _mapper;

    public SearchCompensationOrdersQueryHandler(
        ICompensationOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchCompensationOrdersResponse>> Handle(
        SearchCompensationOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _repository.SearchAsync(
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

        var mappedItems = _mapper.Map<List<SearchCompensationOrdersResponseItem>>(items);

        return Result.Success(new SearchCompensationOrdersResponse(
            mappedItems,
            totalCount,
            request.Page,
            request.PageSize));
    }
}