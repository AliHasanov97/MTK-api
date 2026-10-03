using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Vendors.Queries.GetVendorById;

internal sealed class GetVendorByIdQueryHandler : IQueryHandler<GetVendorByIdQuery, VendorResponse>
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IMapper _mapper;

    public GetVendorByIdQueryHandler(IVendorRepository vendorRepository, IMapper mapper)
    {
        _vendorRepository = vendorRepository;
        _mapper = mapper;
    }

    public async Task<Result<VendorResponse>> Handle(
        GetVendorByIdQuery request,
        CancellationToken cancellationToken)
    {
        var vendor = await _vendorRepository.GetByIdAsync(request.VendorId, cancellationToken);

        if (vendor is null)
        {
            return Result.Failure<VendorResponse>(new Error(
                "Vendor.NotFound",
                $"Tədarükçü tapılmadı: {request.VendorId}"));
        }

        return _mapper.Map<VendorResponse>(vendor);
    }
}
