using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByProperty;

internal sealed class GetPaymentsByPropertyQueryHandler
    : IQueryHandler<GetPaymentsByPropertyQuery, IReadOnlyCollection<PaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentsByPropertyQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<PaymentResponse>>> Handle(
        GetPaymentsByPropertyQuery request,
        CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);

        return _mapper.Map<List<PaymentResponse>>(payments);
    }
}
