using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Customers.Commands.DeleteCustomer;

public sealed record DeleteCustomerCommand(Guid CustomerId) : ICommand;
