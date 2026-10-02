using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByVendor;

public sealed record GetPaymentsByVendorQuery(Guid VendorId) : IQuery<IReadOnlyCollection<PaymentResponse>>;
