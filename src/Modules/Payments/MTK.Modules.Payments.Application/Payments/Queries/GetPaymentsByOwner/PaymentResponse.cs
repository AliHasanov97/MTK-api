using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;

public sealed record PaymentResponse(
    Guid Id,
    Guid OwnerId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    DateTimeOffset PaymentDate,
    PaymentStatus Status,
    string? Reference,
    string? Notes,
    DateTimeOffset CreatedAt);
