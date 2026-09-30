using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Vendors.Queries.GetVendorById;

internal sealed class GetVendorByIdQueryHandler : IQueryHandler<GetVendorByIdQuery, VendorResponse>
{
    private readonly IVendorRepository _vendorRepository;

    public GetVendorByIdQueryHandler(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
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

        return new VendorResponse(
            vendor.Id,
            vendor.Name,
            vendor.VendorType,
            vendor.Voen,
            vendor.Director,
            vendor.Email,
            vendor.Phone,
            vendor.Address,
            vendor.Note,
            vendor.IsActive,
            vendor.CreatedAt,
            vendor.UpdatedAt);
    }
}
