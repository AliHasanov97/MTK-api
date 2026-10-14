using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Customers;

namespace MTK.Modules.Payments.Application.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    Guid CustomerId,
    string Name,
    CustomerType CustomerType,
    string? Voen = null,
    string? Director = null,
    string? Email = null,
    string? Phone = null,
    string? Address = null,
    string? Note = null) : ICommand;
