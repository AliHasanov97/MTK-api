using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Customers.Commands.SetCustomerStatus;

/// <summary>Müştərini aktivləşdirir/dayandırır. Borcu olan müştərini silmək əvəzinə dayandırmaq tövsiyə olunur.</summary>
public sealed record SetCustomerStatusCommand(
    Guid CustomerId,
    bool IsActive) : ICommand;
