using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.EducationLeaveOrders;

namespace MTK.Modules.Hr.Application.EducationLeaveOrders.SearchEducationLeaveOrders;

internal sealed class SearchEducationLeaveOrdersQueryHandler
    : IQueryHandler<SearchEducationLeaveOrdersQuery, SearchEducationLeaveOrdersResponse>
{
    private readonly IEducationLeaveOrderRepository _repository;
    private readonly IMapper _mapper;

    public SearchEducationLeaveOrdersQueryHandler(
        IEducationLeaveOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchEducationLeaveOrdersResponse>> Handle(
        SearchEducationLeaveOrdersQuery request,
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

        var data = _mapper.Map<List<SearchEducationLeaveOrdersResponseItem>>(orders);
        return Result.Success(new SearchEducationLeaveOrdersResponse(data, totalCount, request.Page, request.PageSize));
    }
}
