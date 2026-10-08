using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentById;

public sealed record GetPaymentByIdQuery(Guid PaymentId) : IQuery<PaymentResponse>;
