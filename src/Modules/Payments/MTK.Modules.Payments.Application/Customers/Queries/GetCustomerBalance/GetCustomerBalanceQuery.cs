using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Customers.Queries.GetCustomerBalance;

public sealed record GetCustomerBalanceQuery(Guid CustomerId) : IQuery<CustomerBalanceResponse>;
