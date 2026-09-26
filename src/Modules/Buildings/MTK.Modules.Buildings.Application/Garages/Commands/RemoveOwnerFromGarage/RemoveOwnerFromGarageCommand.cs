using MTK.Common.Application.Messaging;
using System.Text.Json.Serialization;

namespace MTK.Modules.Buildings.Application.Garages.Commands.RemoveOwnerFromGarage;

public sealed class RemoveOwnerFromGarageCommand : ICommand
{
    [JsonIgnore]
    public Guid GarageId { get; set; }
}
