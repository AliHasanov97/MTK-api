using AutoMapper;
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
    private readonly IMapper _mapper;

    public GetPaymentsByVendorQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<PaymentResponse>>> Handle(
        GetPaymentsByVendorQuery request,
        CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetByPartyIdAsync(PartyType.Vendor, request.VendorId, cancellationToken);

        return _mapper.Map<List<PaymentResponse>>(payments);
    }
}
