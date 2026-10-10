using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.GetEmploymentStatusChangeOrderById;

internal sealed class GetEmploymentStatusChangeOrderByIdQueryHandler
    : IQueryHandler<GetEmploymentStatusChangeOrderByIdQuery, GetEmploymentStatusChangeOrderByIdResponse>
{
    private readonly IEmploymentStatusChangeOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetEmploymentStatusChangeOrderByIdQueryHandler(
        IEmploymentStatusChangeOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetEmploymentStatusChangeOrderByIdResponse>> Handle(
        GetEmploymentStatusChangeOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (order == null)
            return Result.Failure<GetEmploymentStatusChangeOrderByIdResponse>(
                EmploymentStatusChangeOrderErrors.NotFound);

        var response = _mapper.Map<GetEmploymentStatusChangeOrderByIdResponse>(order);

        return Result.Success(response);
    }
}
