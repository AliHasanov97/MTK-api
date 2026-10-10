using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.UnpaidLeaveOrders;

namespace MTK.Modules.Hr.Application.UnpaidLeaveOrders.SearchUnpaidLeaveOrders;

internal sealed class SearchUnpaidLeaveOrdersQueryHandler
    : IQueryHandler<SearchUnpaidLeaveOrdersQuery, SearchUnpaidLeaveOrdersResponse>
{
    private readonly IUnpaidLeaveOrderRepository _repository;
    private readonly IMapper _mapper;

    public SearchUnpaidLeaveOrdersQueryHandler(
        IUnpaidLeaveOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchUnpaidLeaveOrdersResponse>> Handle(
        SearchUnpaidLeaveOrdersQuery request,
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

        var data = _mapper.Map<List<SearchUnpaidLeaveOrdersResponseItem>>(orders);
        return Result.Success(new SearchUnpaidLeaveOrdersResponse(data, totalCount, request.Page, request.PageSize));
    }
}
