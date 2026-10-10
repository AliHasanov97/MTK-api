using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.EducationLeaveOrders;

namespace MTK.Modules.Hr.Application.EducationLeaveOrders.GetEducationLeaveOrderById;

internal sealed class GetEducationLeaveOrderByIdQueryHandler
    : IQueryHandler<GetEducationLeaveOrderByIdQuery, GetEducationLeaveOrderByIdResponse>
{
    private readonly IEducationLeaveOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetEducationLeaveOrderByIdQueryHandler(
        IEducationLeaveOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetEducationLeaveOrderByIdResponse>> Handle(
        GetEducationLeaveOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (order == null)
            return Result.Failure<GetEducationLeaveOrderByIdResponse>(
                EducationLeaveOrderErrors.NotFound);

        return Result.Success(_mapper.Map<GetEducationLeaveOrderByIdResponse>(order));
    }
}
