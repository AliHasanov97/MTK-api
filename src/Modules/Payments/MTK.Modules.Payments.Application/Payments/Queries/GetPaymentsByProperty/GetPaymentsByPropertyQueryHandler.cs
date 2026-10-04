using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByProperty;

internal sealed class GetPaymentsByPropertyQueryHandler
    : IQueryHandler<GetPaymentsByPropertyQuery, IReadOnlyCollection<PaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentDisplayEnricher _displayEnricher;
    private readonly IMapper _mapper;

    public GetPaymentsByPropertyQueryHandler(
        IPaymentRepository paymentRepository,
        IPaymentDisplayEnricher displayEnricher,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _displayEnricher = displayEnricher;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<PaymentResponse>>> Handle(
        GetPaymentsByPropertyQuery request,
        CancellationToken cancellationToken)
    {
        var payments = (await _paymentRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken)).ToList();
        var display = await _displayEnricher.ResolveAsync(payments, cancellationToken);

        return _mapper.Map<List<PaymentResponse>>(payments)
            .Select(r => r with
            {
                PartyName = display.GetValueOrDefault(r.Id)?.PartyName,
                PropertyLabel = display.GetValueOrDefault(r.Id)?.PropertyLabel,
            })
            .ToList();
    }
}
