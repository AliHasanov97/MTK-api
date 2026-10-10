using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.OrdersForChangeOfPosition;

namespace MTK.Modules.Hr.Application.OrdersForChangeOfPosition.GetOrderForChangeOfPositionById;

internal sealed class GetOrderForChangeOfPositionByIdQueryHandler
    : IQueryHandler<GetOrderForChangeOfPositionByIdQuery, GetOrderForChangeOfPositionByIdResponse>
{
    private readonly IOrderForChangeOfPositionRepository _repository;
    private readonly IMapper _mapper;

    public GetOrderForChangeOfPositionByIdQueryHandler(
        IOrderForChangeOfPositionRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetOrderForChangeOfPositionByIdResponse>> Handle(
        GetOrderForChangeOfPositionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (order is null)
            return Result.Failure<GetOrderForChangeOfPositionByIdResponse>(
                OrderForChangeOfPositionErrors.NotFound);

        return Result.Success(_mapper.Map<GetOrderForChangeOfPositionByIdResponse>(order));
    }
}
