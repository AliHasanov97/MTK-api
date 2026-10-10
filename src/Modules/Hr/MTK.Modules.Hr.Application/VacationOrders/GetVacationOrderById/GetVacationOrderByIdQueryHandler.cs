using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.VacationOrders;

namespace MTK.Modules.Hr.Application.VacationOrders.GetVacationOrderById;

internal sealed class GetVacationOrderByIdQueryHandler
    : IQueryHandler<GetVacationOrderByIdQuery, GetVacationOrderByIdResponse>
{
    private readonly IVacationOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetVacationOrderByIdQueryHandler(
        IVacationOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetVacationOrderByIdResponse>> Handle(
        GetVacationOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

        if (order == null)
            return Result.Failure<GetVacationOrderByIdResponse>(
                VacationOrderErrors.NotFound);

        return Result.Success(_mapper.Map<GetVacationOrderByIdResponse>(order));
    }
}