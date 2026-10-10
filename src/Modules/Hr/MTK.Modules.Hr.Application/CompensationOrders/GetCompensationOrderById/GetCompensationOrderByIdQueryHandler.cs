using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.CompensationOrders;
using MTK.Modules.Hr.Domain.CompensationOrders;

namespace MTK.Modules.Hr.Application.CompensationOrders.GetCompensationOrderById;

internal sealed class GetCompensationOrderByIdQueryHandler
    : IQueryHandler<GetCompensationOrderByIdQuery, GetCompensationOrderByIdResponse>
{
    private readonly ICompensationOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetCompensationOrderByIdQueryHandler(
        ICompensationOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetCompensationOrderByIdResponse>> Handle(
        GetCompensationOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (order == null)
            return Result.Failure<GetCompensationOrderByIdResponse>(
                CompensationOrderErrors.NotFound(request.Id));

        return Result.Success(_mapper.Map<GetCompensationOrderByIdResponse>(order));
    }
}