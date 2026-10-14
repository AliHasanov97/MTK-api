using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.CustomerCharges.Queries.SearchCustomerCharges;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.CustomerCharges.Queries.GetChargesByCustomer;

internal sealed class GetChargesByCustomerQueryHandler
    : IQueryHandler<GetChargesByCustomerQuery, IReadOnlyCollection<CustomerChargeResponse>>
{
    private readonly ICustomerChargeRepository _customerChargeRepository;
    private readonly IMapper _mapper;

    public GetChargesByCustomerQueryHandler(ICustomerChargeRepository customerChargeRepository, IMapper mapper)
    {
        _customerChargeRepository = customerChargeRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<CustomerChargeResponse>>> Handle(
        GetChargesByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var charges = await _customerChargeRepository.ListByCustomerAsync(request.CustomerId, cancellationToken);

        return _mapper.Map<List<CustomerChargeResponse>>(charges);
    }
}
