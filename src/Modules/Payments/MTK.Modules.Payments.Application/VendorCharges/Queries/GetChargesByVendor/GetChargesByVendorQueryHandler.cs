using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.VendorCharges.Queries.SearchVendorCharges;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.VendorCharges.Queries.GetChargesByVendor;

internal sealed class GetChargesByVendorQueryHandler
    : IQueryHandler<GetChargesByVendorQuery, IReadOnlyCollection<VendorChargeResponse>>
{
    private readonly IVendorChargeRepository _vendorChargeRepository;

    public GetChargesByVendorQueryHandler(IVendorChargeRepository vendorChargeRepository)
    {
        _vendorChargeRepository = vendorChargeRepository;
    }

    public async Task<Result<IReadOnlyCollection<VendorChargeResponse>>> Handle(
        GetChargesByVendorQuery request,
        CancellationToken cancellationToken)
    {
        var charges = await _vendorChargeRepository.ListByVendorAsync(request.VendorId, cancellationToken);

        var response = charges
            .Select(SearchVendorChargesQueryHandler.ToResponse)
            .ToList();

        return response;
    }
}
