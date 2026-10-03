using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;

internal sealed class GetPaymentsByOwnerQueryHandler : IQueryHandler<GetPaymentsByOwnerQuery, IReadOnlyCollection<PaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentsByOwnerQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<PaymentResponse>>> Handle(
        GetPaymentsByOwnerQuery request,
        CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);

        return _mapper.Map<List<PaymentResponse>>(payments);
    }
}
