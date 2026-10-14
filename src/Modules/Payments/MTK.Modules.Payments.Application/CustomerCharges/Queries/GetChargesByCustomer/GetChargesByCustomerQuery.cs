using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.CustomerCharges.Queries.SearchCustomerCharges;

namespace MTK.Modules.Payments.Application.CustomerCharges.Queries.GetChargesByCustomer;

public sealed record GetChargesByCustomerQuery(Guid CustomerId) : IQuery<IReadOnlyCollection<CustomerChargeResponse>>;
