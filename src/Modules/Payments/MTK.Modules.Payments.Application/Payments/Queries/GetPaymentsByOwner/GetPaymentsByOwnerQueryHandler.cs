using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;

internal sealed class GetPaymentsByOwnerQueryHandler : IQueryHandler<GetPaymentsByOwnerQuery, IReadOnlyCollection<PaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentsByOwnerQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<IReadOnlyCollection<PaymentResponse>>> Handle(
        GetPaymentsByOwnerQuery request,
        CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);

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
                p.CreatedAt))
            .ToList();

        return response;
    }
}
