using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Orders;

namespace MTK.Modules.Hr.Application.Orders.SearchOrders;

internal sealed class SearchOrdersQueryHandler(
    IOrderRepository repository,
    IMapper mapper)
    : IQueryHandler<SearchOrdersQuery, SearchOrdersResponse>
{
    public async Task<Result<SearchOrdersResponse>> Handle(
        SearchOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await repository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await repository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var data = mapper.Map<List<SearchOrdersResponseItem>>(orders);

        return Result.Success(new SearchOrdersResponse(
            data,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10));
    }
}