using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.WorkOnNonWorkdayOrders;

namespace MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.SearchWorkOnNonWorkdayOrders;

internal sealed class SearchWorkOnNonWorkdayOrdersQueryHandler
    : IQueryHandler<SearchWorkOnNonWorkdayOrdersQuery, SearchWorkOnNonWorkdayOrdersResponse>
{
    private readonly IWorkOnNonWorkdayOrderRepository _repository;
    private readonly IMapper _mapper;

    public SearchWorkOnNonWorkdayOrdersQueryHandler(
        IWorkOnNonWorkdayOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchWorkOnNonWorkdayOrdersResponse>> Handle(
        SearchWorkOnNonWorkdayOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var workOnNonWorkdayOrders = await _repository.SearchAsync(
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

        var data = _mapper.Map<List<SearchWorkOnNonWorkdayOrdersResponseItem>>(workOnNonWorkdayOrders);
        return Result.Success(new SearchWorkOnNonWorkdayOrdersResponse(data, totalCount, request.Page ?? 1, request.PageSize ?? 10));
    }
}
