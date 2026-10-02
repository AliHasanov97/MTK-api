using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.PropertyOwnerships;

namespace MTK.Modules.Payments.Application.PropertyOwnerships.Commands.SyncPropertyOwnership;

public sealed record SyncPropertyOwnershipCommand(
    Guid PropertyId,
    PropertyType PropertyType,
    Guid OwnerId,
    decimal AreaSquareMeters,
    GarageType? GarageType = null,
    string? PropertyNumber = null) : ICommand;
