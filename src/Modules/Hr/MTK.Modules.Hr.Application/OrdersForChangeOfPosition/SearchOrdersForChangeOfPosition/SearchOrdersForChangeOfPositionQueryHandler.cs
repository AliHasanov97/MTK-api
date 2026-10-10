using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.OrdersForChangeOfPosition;

namespace MTK.Modules.Hr.Application.OrdersForChangeOfPosition.SearchOrdersForChangeOfPosition;

internal sealed class SearchOrdersForChangeOfPositionQueryHandler
    : IQueryHandler<SearchOrdersForChangeOfPositionQuery, SearchOrdersForChangeOfPositionResponse>
{
    private readonly IOrderForChangeOfPositionRepository _repository;
    private readonly IMapper _mapper;

    public SearchOrdersForChangeOfPositionQueryHandler(
        IOrderForChangeOfPositionRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchOrdersForChangeOfPositionResponse>> Handle(
        SearchOrdersForChangeOfPositionQuery request,
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

        var data = _mapper.Map<List<SearchOrdersForChangeOfPositionResponseItem>>(orders);

        return Result.Success(new SearchOrdersForChangeOfPositionResponse(data, totalCount, request.Page ?? 0, request.PageSize ?? 100));
    }
}
