using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.VendorCharges.Queries.SearchVendorCharges;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.VendorCharges.Queries.GetChargesByVendor;

internal sealed class GetChargesByVendorQueryHandler
    : IQueryHandler<GetChargesByVendorQuery, IReadOnlyCollection<VendorChargeResponse>>
{
    private readonly IVendorChargeRepository _vendorChargeRepository;
    private readonly IMapper _mapper;

    public GetChargesByVendorQueryHandler(IVendorChargeRepository vendorChargeRepository, IMapper mapper)
    {
        _vendorChargeRepository = vendorChargeRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<VendorChargeResponse>>> Handle(
        GetChargesByVendorQuery request,
        CancellationToken cancellationToken)
    {
        var charges = await _vendorChargeRepository.ListByVendorAsync(request.VendorId, cancellationToken);

        return _mapper.Map<List<VendorChargeResponse>>(charges);
    }
}
