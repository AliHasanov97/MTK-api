using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentById;

internal sealed class GetPaymentByIdQueryHandler : IQueryHandler<GetPaymentByIdQuery, PaymentResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentDisplayEnricher _displayEnricher;
    private readonly IMapper _mapper;

    public GetPaymentByIdQueryHandler(
        IPaymentRepository paymentRepository,
        IPaymentDisplayEnricher displayEnricher,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _displayEnricher = displayEnricher;
        _mapper = mapper;
    }

    public async Task<Result<PaymentResponse>> Handle(
        GetPaymentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdDefaultAsync(request.PaymentId, cancellationToken);
        if (payment is null)
        {
            return Result.Failure<PaymentResponse>(new Error(
                "Payment.NotFound",
                $"Ödəniş tapılmadı: {request.PaymentId}"));
        }

        var display = await _displayEnricher.ResolveAsync([payment], cancellationToken);
        var response = _mapper.Map<PaymentResponse>(payment);
        return response with
        {
            PartyName = display.GetValueOrDefault(payment.Id)?.PartyName,
            PropertyLabel = display.GetValueOrDefault(payment.Id)?.PropertyLabel,
        };
    }
}
