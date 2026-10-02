using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.VendorCharges.Queries.SearchVendorCharges;

namespace MTK.Modules.Payments.Application.VendorCharges.Queries.GetChargesByVendor;

public sealed record GetChargesByVendorQuery(Guid VendorId) : IQuery<IReadOnlyCollection<VendorChargeResponse>>;
