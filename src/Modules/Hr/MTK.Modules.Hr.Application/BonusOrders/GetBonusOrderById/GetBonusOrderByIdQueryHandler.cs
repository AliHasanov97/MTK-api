using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.BonusOrders;

namespace MTK.Modules.Hr.Application.BonusOrders.GetBonusOrderById;

internal sealed class GetBonusOrderByIdQueryHandler
    : IQueryHandler<GetBonusOrderByIdQuery, GetBonusOrderByIdResponse>
{
    private readonly IBonusOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetBonusOrderByIdQueryHandler(
        IBonusOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetBonusOrderByIdResponse>> Handle(
        GetBonusOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var bonusOrder = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (bonusOrder is null)
        {
            return Result.Failure<GetBonusOrderByIdResponse>(BonusOrderErrors.NotFound);
        }

        return Result.Success(_mapper.Map<GetBonusOrderByIdResponse>(bonusOrder));
    }
}
