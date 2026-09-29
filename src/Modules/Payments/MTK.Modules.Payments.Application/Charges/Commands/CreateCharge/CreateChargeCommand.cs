using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Application.Charges.Commands.CreateCharge;

/// <summary>
/// Creates a one-off/manual charge (penalty, repair fee, etc.) outside the monthly
/// recurring generation. <paramref name="Period"/> is optional — when omitted, a value
/// distinct from the "yyyy-MM" period used by auto-generated charges is created so it
/// never collides with the (OwnerId, PropertyId, Period) uniqueness constraint.
/// </summary>
public sealed record CreateChargeCommand(
    Guid OwnerId,
    PropertyType PropertyType,
    Guid PropertyId,
    decimal Amount,
    string Description,
    string? Period = null) : ICommand<Guid>;
