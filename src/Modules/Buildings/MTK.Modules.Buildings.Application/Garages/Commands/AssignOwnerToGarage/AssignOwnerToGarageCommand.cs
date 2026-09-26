using MTK.Common.Application.Messaging;
using System.Text.Json.Serialization;

namespace MTK.Modules.Buildings.Application.Garages.Commands.AssignOwnerToGarage;

public sealed class AssignOwnerToGarageCommand : ICommand
{
    public Guid OwnerId { get; set; }

    [JsonIgnore]
    public Guid GarageId { get; set; }
}
