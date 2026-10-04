using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.PropertyOwnerships.Commands.SyncPropertyOwnership;

/// <summary>Exactly one of ApartmentId/GarageId must be set.</summary>
public sealed record SyncPropertyOwnershipCommand(
    Guid? ApartmentId,
    Guid? GarageId,
    Guid OwnerId) : ICommand;
