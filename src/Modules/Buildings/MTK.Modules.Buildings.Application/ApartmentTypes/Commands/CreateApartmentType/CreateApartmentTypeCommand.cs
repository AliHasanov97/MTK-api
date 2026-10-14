using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.ApartmentTypes.Commands.CreateApartmentType;

public sealed record CreateApartmentTypeCommand(
    int RoomCount,
    decimal AreaSquareMeters) : ICommand<Guid>
{
    /// <summary>Route-dan set olunur.</summary>
    public Guid BuildingId { get; set; }
}
