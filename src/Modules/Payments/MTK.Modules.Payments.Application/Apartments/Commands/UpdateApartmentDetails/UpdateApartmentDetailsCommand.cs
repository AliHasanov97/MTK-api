using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Apartments.Commands.UpdateApartmentDetails;

/// <summary>
/// For Buildings events that carry an apartment's current number/area but no
/// BuildingId (ApartmentUpdated, ApartmentOwnerChanged) — unlike SyncApartmentCommand,
/// this never touches the Building shadow, so it can't accidentally create a bogus
/// Building row from a missing/placeholder BuildingId. Only updates an apartment shadow
/// that already exists; if it doesn't (ApartmentCreated hasn't synced yet), it's a
/// no-op — ApartmentCreated will bring the up-to-date details anyway.
/// ApartmentNumber null means "leave the existing number as-is" (ApartmentUpdated
/// doesn't carry one).
/// </summary>
public sealed record UpdateApartmentDetailsCommand(
    Guid ApartmentId,
    string? ApartmentNumber,
    decimal AreaSquareMeters) : ICommand;
