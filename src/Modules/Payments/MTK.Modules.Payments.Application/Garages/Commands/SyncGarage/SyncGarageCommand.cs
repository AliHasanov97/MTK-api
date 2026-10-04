using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.PropertyOwnerships;

namespace MTK.Modules.Payments.Application.Garages.Commands.SyncGarage;

public sealed record SyncGarageCommand(Guid GarageId, string GarageNumber, GarageType GarageType) : ICommand;
