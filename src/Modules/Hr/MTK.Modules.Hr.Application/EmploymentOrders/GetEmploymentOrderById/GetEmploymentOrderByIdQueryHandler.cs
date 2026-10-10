using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.EmploymentOrders;

namespace MTK.Modules.Hr.Application.EmploymentOrders.GetEmploymentOrderById;

internal sealed class GetEmploymentOrderByIdQueryHandler : IQueryHandler<GetEmploymentOrderByIdQuery, GetEmploymentOrderByIdResponse>
{
    private readonly IEmploymentOrderRepository _employmentOrderRepository;
    private readonly IMapper _mapper;

    public GetEmploymentOrderByIdQueryHandler(IEmploymentOrderRepository employmentOrderRepository, IMapper mapper)
    {
        _employmentOrderRepository = employmentOrderRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetEmploymentOrderByIdResponse>> Handle(GetEmploymentOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _employmentOrderRepository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (order is null)
            return Result.Failure<GetEmploymentOrderByIdResponse>(EmploymentOrderErrors.NotFound);

        return Result.Success(_mapper.Map<GetEmploymentOrderByIdResponse>(order));
    }
}
