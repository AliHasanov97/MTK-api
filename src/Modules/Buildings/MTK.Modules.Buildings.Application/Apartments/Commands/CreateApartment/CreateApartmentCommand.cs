using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Apartments.Commands.CreateApartment;

public sealed record CreateApartmentCommand(
    Guid BuildingId,
    string ApartmentNumber,
    int Floor,
    decimal AreaSquareMeters,
    int RoomCount) : ICommand<Guid>;
