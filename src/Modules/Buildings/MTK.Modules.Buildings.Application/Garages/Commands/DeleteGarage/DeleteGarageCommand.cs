using MTK.Common.Application.Messaging;
using System.Text.Json.Serialization;

namespace MTK.Modules.Buildings.Application.Garages.Commands.DeleteGarage;

public sealed class DeleteGarageCommand : ICommand
{
    [JsonIgnore]
    public Guid GarageId { get; set; }
}
