using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Application.Charges.Queries.GetChargesByOwner;

public sealed record ChargeResponse(
    Guid Id,
    Guid OwnerId,
    PropertyType PropertyType,
    Guid PropertyId,
    string Period,
    decimal Amount,
    decimal PaidAmount,
    ChargeStatus Status,
    DateTimeOffset CreatedAt);
