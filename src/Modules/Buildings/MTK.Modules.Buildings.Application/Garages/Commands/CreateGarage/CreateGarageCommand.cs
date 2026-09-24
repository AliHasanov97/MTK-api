using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Garages.Commands.CreateGarage;

public sealed record CreateGarageCommand(
    Guid? OwnerId,
    string GarageNumber,
    string GarageType, // "OpenParking", "CoveredGarage", "Storage"
    string? Description = null) : ICommand<Guid>;
