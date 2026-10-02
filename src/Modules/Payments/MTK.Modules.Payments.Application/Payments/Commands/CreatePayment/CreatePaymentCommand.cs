using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payments.Commands.CreatePayment;

// No client-supplied PaymentDate: a payment's moment is when it was recorded,
// not something a caller should be able to backdate/forge — the handler stamps
// it with DateTimeOffset.UtcNow.
public sealed record CreatePaymentCommand(
    Guid OwnerId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    string? Reference,
    string? Notes,
    // Optional: scope this payment to one apartment/garage instead of FIFO across
    // everything the owner owes. When set, PropertyType must be set too.
    Guid? PropertyId = null,
    PropertyType? PropertyType = null) : ICommand<Guid>;
