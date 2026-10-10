using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.VacationReturnOrders;

namespace MTK.Modules.Hr.Application.VacationReturnOrders.GetVacationReturnOrderById;

internal sealed class GetVacationReturnOrderByIdQueryHandler
    : IQueryHandler<GetVacationReturnOrderByIdQuery, GetVacationReturnOrderByIdResponse>
{
    private readonly IVacationReturnOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetVacationReturnOrderByIdQueryHandler(
        IVacationReturnOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetVacationReturnOrderByIdResponse>> Handle(
        GetVacationReturnOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

        if (order == null)
            return Result.Failure<GetVacationReturnOrderByIdResponse>(
                VacationReturnOrderErrors.NotFound);

        return Result.Success(_mapper.Map<GetVacationReturnOrderByIdResponse>(order));
    }
}