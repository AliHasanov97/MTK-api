using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payments.Commands.CreatePayment;

public sealed record CreatePaymentCommand(
    Guid OwnerId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    DateTimeOffset PaymentDate,
    string? Reference,
    string? Notes) : ICommand<Guid>;
