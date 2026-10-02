using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByVendor;

internal sealed class GetPaymentsByVendorQueryHandler
    : IQueryHandler<GetPaymentsByVendorQuery, IReadOnlyCollection<PaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentsByVendorQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<IReadOnlyCollection<PaymentResponse>>> Handle(
        GetPaymentsByVendorQuery request,
        CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetByPartyIdAsync(PartyType.Vendor, request.VendorId, cancellationToken);

        var response = payments
            .Select(p => new PaymentResponse(
                p.Id,
                p.PartyId,
                p.Amount,
                p.PaymentMethod,
                p.PaymentDate,
                p.Status,
                p.Notes,
                p.CreatedAt,
                p.PropertyId,
                p.PropertyType))
            .ToList();

        return response;
    }
}
