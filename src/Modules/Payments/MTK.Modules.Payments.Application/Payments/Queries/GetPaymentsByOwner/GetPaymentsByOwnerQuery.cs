using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;

public sealed record GetPaymentsByOwnerQuery(Guid OwnerId) : IQuery<IReadOnlyCollection<PaymentResponse>>;
