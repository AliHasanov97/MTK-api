using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByProperty;

internal sealed class GetPaymentsByPropertyQueryHandler
    : IQueryHandler<GetPaymentsByPropertyQuery, IReadOnlyCollection<PaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentsByPropertyQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<IReadOnlyCollection<PaymentResponse>>> Handle(
        GetPaymentsByPropertyQuery request,
        CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);

        var response = payments
            .Select(p => new PaymentResponse(
                p.Id,
                p.OwnerId,
                p.Amount,
                p.PaymentMethod,
                p.PaymentDate,
                p.Status,
                p.Reference,
                p.Notes,
                p.CreatedAt,
                p.PropertyId,
                p.PropertyType))
            .ToList();

        return response;
    }
}
