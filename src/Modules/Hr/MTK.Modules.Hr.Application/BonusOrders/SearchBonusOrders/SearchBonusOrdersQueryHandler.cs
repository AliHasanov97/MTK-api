using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.BonusOrders;

namespace MTK.Modules.Hr.Application.BonusOrders.SearchBonusOrders;

internal sealed class SearchBonusOrdersQueryHandler
    : IQueryHandler<SearchBonusOrdersQuery, SearchBonusOrdersResponse>
{
    private readonly IBonusOrderRepository _repository;
    private readonly IMapper _mapper;

    public SearchBonusOrdersQueryHandler(
        IBonusOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchBonusOrdersResponse>> Handle(
        SearchBonusOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var bonusOrders = await _repository.SearchAsync(
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

        var data = _mapper.Map<List<SearchBonusOrdersResponseItem>>(bonusOrders);
        return Result.Success(new SearchBonusOrdersResponse(data, totalCount, request.Page ?? 1, request.PageSize ?? 10));
    }
}
