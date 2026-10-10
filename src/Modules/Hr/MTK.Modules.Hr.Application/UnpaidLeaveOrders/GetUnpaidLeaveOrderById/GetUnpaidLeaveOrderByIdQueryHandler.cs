using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.UnpaidLeaveOrders;

namespace MTK.Modules.Hr.Application.UnpaidLeaveOrders.GetUnpaidLeaveOrderById;

internal sealed class GetUnpaidLeaveOrderByIdQueryHandler
    : IQueryHandler<GetUnpaidLeaveOrderByIdQuery, GetUnpaidLeaveOrderByIdResponse>
{
    private readonly IUnpaidLeaveOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetUnpaidLeaveOrderByIdQueryHandler(
        IUnpaidLeaveOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetUnpaidLeaveOrderByIdResponse>> Handle(
        GetUnpaidLeaveOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (order == null)
            return Result.Failure<GetUnpaidLeaveOrderByIdResponse>(
                UnpaidLeaveOrderErrors.NotFound);

        return Result.Success(_mapper.Map<GetUnpaidLeaveOrderByIdResponse>(order));
    }
}
