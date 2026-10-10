using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.WorkOnNonWorkdayOrders;

namespace MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.GetWorkOnNonWorkdayOrderById;

internal sealed class GetWorkOnNonWorkdayOrderByIdQueryHandler
    : IQueryHandler<GetWorkOnNonWorkdayOrderByIdQuery, GetWorkOnNonWorkdayOrderByIdResponse>
{
    private readonly IWorkOnNonWorkdayOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetWorkOnNonWorkdayOrderByIdQueryHandler(
        IWorkOnNonWorkdayOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetWorkOnNonWorkdayOrderByIdResponse>> Handle(
        GetWorkOnNonWorkdayOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var workOnNonWorkdayOrder = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (workOnNonWorkdayOrder is null)
        {
            return Result.Failure<GetWorkOnNonWorkdayOrderByIdResponse>(WorkOnNonWorkdayOrderErrors.NotFound);
        }

        return Result.Success(_mapper.Map<GetWorkOnNonWorkdayOrderByIdResponse>(workOnNonWorkdayOrder));
    }
}
