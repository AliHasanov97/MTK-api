using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payments.Commands.CreatePayment;

public sealed record CreatePaymentCommand(
    Guid OwnerId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    DateTimeOffset PaymentDate,
    string? Reference,
    string? Notes,
    // Optional: scope this payment to one apartment/garage instead of FIFO across
    // everything the owner owes. When set, PropertyType must be set too.
    Guid? PropertyId = null,
    PropertyType? PropertyType = null) : ICommand<Guid>;
