using MTK.Common.Application.Messaging;
using System.Text.Json.Serialization;

namespace MTK.Modules.Buildings.Application.Apartments.Commands.UpdateApartment;

public sealed class UpdateApartmentCommand : ICommand
{
    public decimal AreaSquareMeters { get; set; }
    public int RoomCount { get; set; }

    [JsonIgnore]
    public Guid ApartmentId { get; set; }
}
