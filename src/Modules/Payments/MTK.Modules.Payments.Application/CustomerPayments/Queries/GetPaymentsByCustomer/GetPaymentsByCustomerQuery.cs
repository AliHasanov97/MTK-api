using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;

namespace MTK.Modules.Payments.Application.CustomerPayments.Queries.GetPaymentsByCustomer;

public sealed record GetPaymentsByCustomerQuery(Guid CustomerId) : IQuery<IReadOnlyCollection<PaymentResponse>>;
