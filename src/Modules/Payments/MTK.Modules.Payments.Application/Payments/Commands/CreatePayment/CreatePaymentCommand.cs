using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payments.Commands.CreatePayment;

// No client-supplied PaymentDate: a payment's moment is when it was recorded,
// not something a caller should be able to backdate/forge — the handler stamps
// it with DateTimeOffset.UtcNow.
public sealed record CreatePaymentCommand(
    Guid OwnerId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    string? Notes,
    // Optional: scope this payment to one apartment/garage instead of FIFO across
    // everything the owner owes. At most one of the two may be set.
    Guid? ApartmentId = null,
    Guid? GarageId = null) : ICommand<Guid>;
