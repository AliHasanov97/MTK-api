using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Charges.Commands.CreateCharge;

/// <summary>
/// Creates a one-off/manual charge (penalty, repair fee, etc.) outside the monthly
/// recurring generation. Exactly one of <paramref name="ApartmentId"/>/
/// <paramref name="GarageId"/> must be set. <paramref name="Period"/> is optional —
/// when omitted, a value distinct from the "yyyy-MM" period used by auto-generated
/// charges is created so it never collides with the (OwnerId, Apartment/GarageId,
/// Period) uniqueness constraint.
/// </summary>
public sealed record CreateChargeCommand(
    Guid OwnerId,
    Guid? ApartmentId,
    Guid? GarageId,
    decimal Amount,
    string Description,
    string? Period = null) : ICommand<Guid>;
