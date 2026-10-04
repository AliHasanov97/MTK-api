using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Apartments.Commands.SyncApartment;

public sealed record SyncApartmentCommand(
    Guid ApartmentId,
    Guid BuildingId,
    string ApartmentNumber,
    decimal AreaSquareMeters,
    string BuildingName,
    string BuildingAddress) : ICommand;
