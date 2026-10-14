using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.CustomerPayments.Queries.GetPaymentsByCustomer;

internal sealed class GetPaymentsByCustomerQueryHandler
    : IQueryHandler<GetPaymentsByCustomerQuery, IReadOnlyCollection<PaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public GetPaymentsByCustomerQueryHandler(
        IPaymentRepository paymentRepository,
        ICustomerRepository customerRepository,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<PaymentResponse>>> Handle(
        GetPaymentsByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var payments = (await _paymentRepository.GetByPartyIdAsync(PartyType.Customer, request.CustomerId, cancellationToken)).ToList();
        var customer = await _customerRepository.GetByIdDefaultAsync(request.CustomerId, cancellationToken);

        return _mapper.Map<List<PaymentResponse>>(payments)
            .Select(r => r with { PartyName = customer?.Name })
            .ToList();
    }
}
