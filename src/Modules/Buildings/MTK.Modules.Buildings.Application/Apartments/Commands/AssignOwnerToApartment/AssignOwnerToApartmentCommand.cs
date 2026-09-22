using MTK.Common.Application.Messaging;
using System.Text.Json.Serialization;

namespace MTK.Modules.Buildings.Application.Apartments.Commands.AssignOwnerToApartment;

public sealed class AssignOwnerToApartmentCommand : ICommand
{
    public Guid OwnerId { get; set; }

    [JsonIgnore]
    public Guid ApartmentId { get; set; }
}
