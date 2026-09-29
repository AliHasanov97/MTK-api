using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByProperty;

public sealed record GetPaymentsByPropertyQuery(Guid PropertyId) : IQuery<IReadOnlyCollection<PaymentResponse>>;
