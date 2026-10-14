using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(Guid CustomerId) : IQuery<CustomerResponse>;
