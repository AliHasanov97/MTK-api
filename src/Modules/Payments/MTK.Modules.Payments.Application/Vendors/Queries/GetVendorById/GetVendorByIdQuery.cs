using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Vendors.Queries.GetVendorById;

public sealed record GetVendorByIdQuery(Guid VendorId) : IQuery<VendorResponse>;
